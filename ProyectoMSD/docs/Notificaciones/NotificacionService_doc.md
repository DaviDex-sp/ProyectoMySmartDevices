# NotificacionService — Documentación del Módulo

## Propósito

`NotificacionService` es el servicio central responsable de persistir notificaciones automáticas del sistema en la tabla `notificaciones` de la base de datos. Es invocado por eventos de negocio específicos y expone dos operaciones:

1. **Notificación individual** (`CrearAsync`): persiste una notificación para un usuario específico.
2. **Broadcast por rol** (`CrearParaRolAsync`): persiste una notificación para todos los usuarios que posean un rol determinado (ej. todos los Administradores).

Este servicio desacopla la lógica de notificaciones de sus invocadores (`DispositivoService`, `MqttDomoticaService`), siguiendo el Principio de Responsabilidad Única y respetando los límites de la Arquitectura Limpia.

---

## Dependencias

| Dependencia | Tipo | Ciclo de vida | Propósito |
|---|---|---|---|
| `AppDbContext` | EF Core DbContext | Scoped | Persistencia en tabla `notificaciones` |
| `ILogger<NotificacionService>` | ASP.NET Core Logging | Scoped | Registro estructurado para auditoría y diagnóstico |

**Registro en DI** (`Program.cs`):
```csharp
builder.Services.AddScoped<INotificacionService, NotificacionService>();
```

> **Restricción de ciclo de vida**: debe registrarse como `Scoped`. NO registrar como `Singleton` — depende de `AppDbContext` que es `Scoped`.

---

## API Pública / Interfaz

### `CrearAsync(CrearNotificacionDto dto)`

```csharp
Task CrearAsync(CrearNotificacionDto dto);
```

| Parámetro | Tipo | Descripción |
|---|---|---|
| `dto.IdUsuario` | `int` | ID del usuario destinatario |
| `dto.Titulo` | `string` | Título corto de la notificación (máx. 255 caracteres) |
| `dto.Mensaje` | `string` | Cuerpo descriptivo del evento |
| `dto.Tipo` | `string` | Categoría: `"configuracion"`, `"telemetria"`, `"alerta"`, `"info"` |
| `dto.RutaRedireccion` | `string?` | Ruta relativa de redirección al hacer clic (opcional) |

**Comportamiento**: crea una entidad `Notificacion` con `Leida = false` y `FechaCreacion = DateTime.UtcNow`. Lanza excepción en fallo de BD tras registrar el error en el log.

---

### `CrearParaRolAsync(string rol, string titulo, string mensaje, string tipo, string? ruta)`

```csharp
Task CrearParaRolAsync(string rol, string titulo, string mensaje, string tipo, string? ruta = null);
```

| Parámetro | Tipo | Descripción |
|---|---|---|
| `rol` | `string` | Rol destino (ej. `"Admin"`) |
| `titulo` | `string` | Título de la notificación |
| `mensaje` | `string` | Cuerpo de la notificación |
| `tipo` | `string` | Clasificador de categoría |
| `ruta` | `string?` | Ruta de redirección opcional |

**Comportamiento**: consulta todos los IDs de usuarios con el rol indicado, inserta las notificaciones en bloque vía `AddRange` y llama `SaveChangesAsync`. Si no existen usuarios con el rol, registra una advertencia en el log y retorna sin error.

---

## Eventos que Disparan este Servicio

| Evento | Invocado desde | Método utilizado | Tipo | Destinatarios |
|---|---|---|---|---|
| Dispositivo creado exitosamente | `DispositivoService.CreateAsync` | `CrearAsync` | `"configuracion"` | Usuario autenticado creador |
| Primer mensaje MQTT por tópico | `MqttDomoticaService` | `CrearParaRolAsync` | `"telemetria"` | Todos los usuarios con rol `"Admin"` |

---

## Ejemplo de Uso

### Crear notificación para un usuario

```csharp
await _notificacionService.CrearAsync(new CrearNotificacionDto
{
    IdUsuario       = idUsuarioActual,
    Titulo          = "Dispositivo Configurado",
    Mensaje         = "El sensor ha sido registrado correctamente.",
    Tipo            = "configuracion",
    RutaRedireccion = "/Dispositivos/Details?id=42"
});
```

### Broadcast a un rol (desde un Singleton)

```csharp
using var scope = _scopeFactory.CreateScope();
var svc = scope.ServiceProvider.GetRequiredService<INotificacionService>();

await svc.CrearParaRolAsync(
    rol:     "Admin",
    titulo:  "Telemetría Activa",
    mensaje: "El sensor en 'domotica/sensores/clima' ha comenzado a transmitir.",
    tipo:    "telemetria",
    ruta:    "/Dispositivos"
);
```

---

## Notas Técnicas

- `FechaCreacion` siempre se asigna con `DateTime.UtcNow` en el servidor. Nunca se confía en fechas enviadas por el cliente.
- `Leida` siempre se inicializa en `false`. El usuario la marca como leída desde la interfaz.
- Invocar este servicio desde un `Singleton` (ej. `MqttDomoticaService`) **requiere** `IServiceScopeFactory`. La inyección directa en el constructor causará una excepción de ciclo de vida en tiempo de ejecución.
