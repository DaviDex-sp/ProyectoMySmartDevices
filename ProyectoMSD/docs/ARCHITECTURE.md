# 🏗️ Arquitectura del Proyecto — MySmartDevice
> **Versión:** 2.1.0 — Actualizado: Abril 2026
> **Responsable:** Arquitecto de Software (arch-1)
> **LINE ENDING MANDATE:** Todos los archivos del repositorio usan Windows CRLF (`\r\n`). LF puro está estrictamente prohibido.

---

## 1. Visión General

**MySmartDevice** es una plataforma web de gestión domótica cloud-native construida sobre **ASP.NET Core 9 Razor Pages**. La arquitectura sigue el patrón **N-Tier / Clean Architecture** con separación estricta de capas:

```
Presentación (Razor Pages / API Controllers)
        ↓ solo vía Interfaces (DIP)
Lógica de Negocio (Services)
        ↓ solo vía AppDbContext
Acceso a Datos (EF Core + MySQL / Aiven)
```

El proyecto está **desplegado en Azure App Services** con base de datos **MySQL 8.0 gestionada por Aiven**, integración IoT vía **HiveMQ MQTT Broker** y comunicación en tiempo real con **SignalR (WebSockets)**.

---

## 2. Stack Tecnológico

| Capa | Tecnología |
|---|---|
| Framework Web | ASP.NET Core 9 — Razor Pages |
| ORM / Acceso a Datos | Entity Framework Core 9 (Pomelo MySQL Driver) |
| Base de Datos | MySQL 8.0 — Aiven Cloud |
| Autenticación | Cookie Auth + OAuth2 Google |
| IoT Broker | HiveMQ (protocolo MQTT 5.0 — MQTTNet) |
| Tiempo Real (Push) | ASP.NET Core SignalR (WebSockets) |
| API REST | ASP.NET Core API Controllers |
| Despliegue | Azure App Services / Docker |
| Secretos | Secret Manager (Dev) / Azure Key Vault (Prod) |

---

## 3. Estructura de Carpetas

```
ProyectoMSD/
│
├── 📂 Controllers/               ← Capa REST API (thin orchestrators)
│   └── 📂 Api/
│       └── MqttComandosController.cs   ← Endpoint de comandos IoT
│
├── 📂 Filters/                   ← Middleware de páginas (interceptores globales)
│   └── NotificacionNavbarFilter.cs     ← Inyecta conteo de notificaciones en ViewData
│
├── 📂 Helpers/                   ← Utilidades transversales (sin estado)
│
├── 📂 Hubs/                      ← SignalR WebSocket Hubs
│   └── DispositivoHub.cs               ← Canal bidireccional de telemetría IoT
│
├── 📂 Interfaces/                ← Contratos de la BLL (Dependency Inversion)
│   ├── IConfiguracionService.cs
│   ├── IDashboardService.cs
│   ├── IDispositivoService.cs
│   ├── IEspacioService.cs
│   ├── IMqttPublisherService.cs
│   ├── IPropiedadService.cs
│   ├── ISoporteService.cs
│   └── IUsuarioService.cs
│
├── 📂 Migrations/                ← Historial de migraciones EF Core
│
├── 📂 Modelos/                   ← Capa de Datos
│   ├── AppDbContext.cs           ← Configuración EF Core (Unit of Work)
│   ├── Usuario.cs
│   ├── Dispositivo.cs
│   ├── Espacio.cs
│   ├── Propiedade.cs
│   ├── Configuracione.cs
│   ├── Soporte.cs
│   ├── Notificacion.cs
│   ├── RegistroAcceso.cs
│   ├── Roles.cs
│   ├── Ubicacione.cs
│   ├── UsuariosPropiedade.cs
│   └── 📂 DTOs/                  ← Data Transfer Objects (contratos de red)
│       ├── ClienteMetricasDto.cs
│       ├── ComandoDispositivoDto.cs
│       ├── DashboardMetricsDto.cs
│       ├── DispositivoDto.cs
│       ├── EspacioDto.cs
│       ├── PagedResultDto.cs
│       ├── PropiedadDto.cs
│       ├── SensorPayloadDto.cs
│       └── SoporteDto.cs
│
├── 📂 Pages/                     ← Capa de Presentación (Razor Pages)
│   ├── 📂 Shared/                ← Layout base, partials globales
│   ├── 📂 Configuraciones/       ← CRUD de configuraciones por usuario
│   ├── 📂 Dashboard/             ← Panel administrativo con Big Data
│   ├── 📂 Dispositivos/          ← Gestión y control de dispositivos IoT
│   ├── 📂 Espacios/              ← Gestión de espacios/habitaciones
│   ├── 📂 Notificaciones/        ← Centro de notificaciones
│   ├── 📂 Perfil/                ← Perfil de usuario y accesibilidad
│   ├── 📂 Propiedades/           ← Gestión de propiedades/inmuebles
│   ├── 📂 Soportes/              ← Tickets de soporte técnico
│   ├── 📂 Ubicaciones/           ← Gestión de ubicaciones geográficas
│   ├── 📂 Usuarios/              ← Administración de usuarios
│   ├── Login.cshtml / .cs        ← Auth local + OAuth2 Google
│   ├── Registrar.cshtml / .cs    ← Registro con sanitización
│   └── Index.cshtml / .cs        ← Landing page
│
├── 📂 Services/                  ← Implementaciones de la BLL
│   ├── ConfiguracionService.cs
│   ├── DashboardService.cs
│   ├── DispositivoService.cs
│   ├── EspacioService.cs
│   ├── MqttDomoticaService.cs    ← BackgroundService (suscriptor MQTT)
│   ├── MqttPublisherService.cs   ← Singleton (publicador MQTT)
│   ├── PropiedadService.cs
│   ├── SoporteService.cs
│   └── UsuarioService.cs
│
├── 📂 docs/                      ← Documentación técnica del proyecto
│   ├── ARCHITECTURE.md           ← Este archivo
│   ├── Documentacion_ProyectoMSD.md
│   ├── 📂 architecture/          ← Blueprints por módulo
│   ├── 📂 MQTT/                  ← Documentación IoT bidireccional
│   ├── 📂 Propiedades/
│   └── 📂 Usuarios/
│
├── 📂 wwwroot/                   ← Assets públicos (CSS, JS, imágenes)
│   ├── 📂 css/
│   │   ├── common-styles.css     ← Design system global (variables, temas)
│   │   ├── site.css
│   │   └── 📂 pages/            ← CSS por módulo (carga bajo demanda)
│   ├── 📂 js/                   ← Scripts frontend compartidos
│   ├── 📂 images/               ← Recursos gráficos fijos
│   └── 📂 uploads/              ← Archivos subidos (avatares, etc.)
│
├── appsettings.json              ← Config no sensible (estructura)
├── appsettings.Development.json  ← Overrides locales (NO commitear secretos)
└── Program.cs                    ← Composición DI + Pipeline HTTP
```

---

## 4. Principios Arquitectónicos Mandatorios

### 4.1 Prohibición del Anti-Patrón "Smart UI"
El `AppDbContext` **NUNCA** se inyecta directamente en `PageModel` ni en `Controllers`. El flujo obligatorio es:

```
PageModel / Controller
    → Interfaz (IXxxService)
        → Service (XxxService)
            → AppDbContext
```

### 4.2 Inversión de Dependencias (DIP)
Todo acoplamiento entre capas ocurre **únicamente a través de interfaces**. Ninguna implementación concreta se referencia desde la capa de presentación.

### 4.3 Secretos y Configuración Cloud-Safe
- **Desarrollo:** `dotnet user-secrets` (Secret Manager).
- **Producción:** Variables de entorno en Azure App Service / Azure Key Vault.
- **Prohibido:** Credenciales hardcodeadas en cualquier archivo del repositorio.

### 4.4 Políticas de Resiliencia (Aiven + HiveMQ)
- EF Core configurado con `EnableRetryOnFailure` (5 intentos, 10 s de espera) contra fallos transitorios de Aiven.
- `MqttPublisherService` implementa reconexión transaccional en caso de caída del broker.
- SignalR configurado con `.withAutomaticReconnect()` en el cliente JS.

---

## 5. Contratos de Servicio (Interfaces — BLL)

### `IUsuarioService`
Gestión completa del ciclo de vida del usuario: autenticación local y OAuth2, CRUD administrativo, trazabilidad de accesos y validaciones.

```csharp
Task<Usuario?> AuthenticateAsync(string correo, string password);
Task<Usuario> AuthenticateGoogleAsync(string email, string name);
Task RegisterAccessAsync(int userId, string action, string? ip, string? userAgent, string path);
Task RegisterLogoutAsync(int userId, string? ip, string? userAgent, string path);
Task<List<Usuario>> GetAllUsuariosAsync();
Task<Usuario?> GetUsuarioByIdAsync(int id);
Task<Usuario?> GetUsuarioPerfilAsync(int userId);
Task CreateUsuarioAsync(Usuario usuario);
Task UpdateUsuarioAsync(Usuario usuario);
Task<bool> UpdatePerfilAsync(int userId, Usuario datosActualizados, string? latitud, string? longitud, string? direccion, string? nuevaClave);
Task DeleteUsuarioAsync(int id);
bool EsEmailValido(string email);
string HashPassword(string password);
Task<bool> ExisteCorreoAsync(string correo);
Task<bool> ExisteDocumentoAsync(string documento);
Task<List<RegistroAcceso>> GetRecentAccessLogsAsync(int? userId, int count);
```

---

### `IDashboardService`
Consultas de agregación (KPIs, Big Data, métricas de clientes) para el panel administrativo. Diseñado para escalar a 10 000+ usuarios sin degradación de rendimiento mediante proyecciones ligeras (`.Select`, `.Count`) en lugar de `ToListAsync()` completo.

```csharp
Task<DashboardMetricsDto> GetMetricsAsync();
Task<DashboardMetricsDto> GetUserMetricsAsync(int userId);
Task<ClienteMetricasDto?> GetClienteMetricasAsync(int clienteId);
Task<PagedResultDto<ClienteResumenDto>> GetResumenClientesAsync(int page, int pageSize, string? busqueda = null);
```

---

### `IPropiedadService`
Lectura de propiedades/inmuebles con filtrado por rol (Admin ve todo; Cliente ve solo las propias).

```csharp
Task<List<PropiedadDto>> GetPropiedadesAsync(bool isAdmin, int? userId);
Task<int> GetTotalPropiedadesAsync();
```

---

### `IEspacioService`
Consulta de espacios con sus dispositivos asociados para renderizado de planos domóticos.

```csharp
Task<List<EspacioDto>> GetEspaciosConDispositivosAsync();
Task<int> GetTotalEspaciosAsync();
```

---

### `IDispositivoService`
Operaciones sobre dispositivos IoT: listado, conteo y alternancia de estado (encendido/apagado).

```csharp
Task<List<DispositivoDto>> GetDispositivosAsync();
Task<int> GetTotalDispositivosAsync();
Task<bool> ToggleEstadoAsync(int id);
```

---

### `IConfiguracionService`
CRUD de configuraciones personalizadas por usuario y métricas auxiliares de widgets de dashboard.

```csharp
Task<IList<Configuracione>> ObtenerTodasAsync();
Task<IList<Configuracione>> ObtenerPorUsuarioAsync(int userId);
Task<Configuracione?> ObtenerPorIdAsync(int id);
Task<bool> CrearAsync(Configuracione configuracion);
Task<bool> ActualizarAsync(Configuracione configuracion);
Task<bool> EliminarAsync(int id);
Task<int> ObtenerConteoNotificacionesAsync();
Task<int> ObtenerConteoDispositivosAsync();
```

---

### `ISoporteService`
Gestión de tickets de soporte con sanitización integrada (anti-XSS) y validación de whitelist de tipos.

```csharp
Task<Soporte?> ObtenerPorIdAsync(int id, bool incluirUsuario = false);
Task<bool> CrearAsync(CrearSoporteDto dto, int usuarioId);
Task<bool> EditarAsync(EditarSoporteDto dto);
Task<bool> ResponderAsync(int soporteId, string respuesta);
string SanitizarTexto(string? input, int maxLength);
bool EsTipoValido(string tipo);
```

---

### `IMqttPublisherService`
Publicación asíncrona de comandos hacia dispositivos IoT a través del Broker HiveMQ.

```csharp
Task<bool> PublishCommandAsync(string topic, string payload);
```

---

## 6. Ciclo de Vida de los Servicios (DI Lifetimes)

| Servicio | Lifetime | Justificación |
|---|---|---|
| `UsuarioService` | `Scoped` | Depende de `AppDbContext` (Scoped por request) |
| `DashboardService` | `Scoped` | Consultas pesadas por petición HTTP |
| `PropiedadService` | `Scoped` | Consultas con filtro por usuario |
| `EspacioService` | `Scoped` | Consultas relacionales por request |
| `DispositivoService` | `Scoped` | Requiere contexto de BD |
| `ConfiguracionService` | `Scoped` | CRUD dependiente del contexto |
| `SoporteService` | `Scoped` | Lógica de negocio + persistencia |
| `MqttDomoticaService` | `Singleton` (HostedService) | BackgroundService con estado de conexión persistente |
| `MqttPublisherService` | `Singleton` | Mantiene pool de sesión MQTT para escrituras |
| `NotificacionNavbarFilter` | `Scoped` | Interceptor global de Razor Pages |

---

## 7. Arquitectura IoT Bidireccional (MQTT + SignalR)

### 7.1 Flujo de Datos — Sensor → UI (Lectura / Push)

```
[ESP32/Sensor]
    → publica en HiveMQ (topic: msd/dispositivos/{id}/telemetria)
        → MqttDomoticaService.cs (BackgroundService — suscriptor)
            → deserializa SensorPayloadDto
                → IHubContext<DispositivoHub>.Clients.All.SendAsync("RecibirTelemetria", payload)
                    → JavaScript SignalR en Details.cshtml actualiza DOM en tiempo real
```

### 7.2 Flujo de Datos — UI → Sensor (Escritura / Control)

```
[Browser — Details.cshtml]
    → fetch() POST a /api/MqttComandos/enviar (ComandoDispositivoDto JSON)
        → MqttComandosController [ApiController] valida y delega
            → IMqttPublisherService.PublishCommandAsync(topic, payload)
                → MqttPublisherService (Singleton) publica en HiveMQ
                    → [ESP32/Dispositivo] recibe el comando y ejecuta la acción
```

### 7.3 DTOs de Transporte IoT

**`SensorPayloadDto`** — Payload entrante del sensor:
```csharp
// Deserializado desde la telemetría MQTT del ESP32
```

**`ComandoDispositivoDto`** — Payload saliente de control:
```csharp
public string TargetTopic { get; set; }  // topic MQTT destino
public string Accion     { get; set; }  // "ON", "OFF", "SET_TEMP", etc.
public string Valor      { get; set; }  // valor opcional del comando
```

---

## 8. Pipeline HTTP — `Program.cs`

### 8.1 Registro de Servicios (orden canónico)

```csharp
// 1. Razor Pages + Filtro Global de Notificaciones
builder.Services.AddRazorPages(opts =>
    opts.Conventions.ConfigureFilter(new ServiceFilterAttribute(typeof(NotificacionNavbarFilter))));
builder.Services.AddScoped<NotificacionNavbarFilter>();

// 2. BLL — Servicios de Negocio (Scoped)
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IConfiguracionService, ConfiguracionService>();
builder.Services.AddScoped<IPropiedadService, PropiedadService>();
builder.Services.AddScoped<IEspacioService, EspacioService>();
builder.Services.AddScoped<IDispositivoService, DispositivoService>();
builder.Services.AddScoped<ISoporteService, SoporteService>();

// 3. IoT — MQTT (BackgroundService + Singleton Publisher)
builder.Services.AddHostedService<MqttDomoticaService>();
builder.Services.AddSingleton<IMqttPublisherService, MqttPublisherService>();

// 4. Tiempo Real y REST API
builder.Services.AddSignalR();
builder.Services.AddControllers();

// 5. Autenticación Múltiple (Cookie + Google OAuth2)
builder.Services.AddAuthentication(/* ... */)
    .AddCookie(/* ... */)
    .AddCookie("ExternalCookie")
    .AddGoogle(/* ClientId/Secret desde IConfiguration */);

// 6. EF Core + MySQL (Aiven) con política de reintentos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connString, ServerVersion.Parse("8.0-mysql"),
        mysql => mysql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));
```

### 8.2 Pipeline de Middleware (orden obligatorio)

```csharp
// Producción: manejo de errores + HSTS
// Desarrollo: DeveloperExceptionPage

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();    // ← SIEMPRE antes de Authorization
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapHub<DispositivoHub>("/hubs/dispositivos");   // SignalR
app.MapControllers();                                // API REST

app.Run();
```

> **⚠️ Regla de orden:** `UseAuthentication` DEBE preceder a `UseAuthorization`. Invertirlos causa fallos silenciosos de autorización.

---

## 9. Arquitectura CSS — Design System

El proyecto aplica un enfoque **Glassmorphism + Dark Mode** con theming basado en CSS Custom Properties (variables). Se prohíben colores hardcodeados en cualquier componente.

```
wwwroot/css/
├── common-styles.css          ← Design system global
│   ├── Variables de color     (--primary, --bg-main, --text-main, etc.)
│   ├── Variables de tema      (light/dark mode via data-theme)
│   ├── Componentes globales   (botones, tarjetas, navbar, glassmorphism)
│   └── Animaciones y micro-interacciones
├── site.css                   ← Resets y defaults base
└── pages/                     ← CSS exclusivo por módulo (lazy load)
    ├── login.css
    ├── propiedades.css
    ├── soportes.css
    ├── notificaciones.css
    └── ...
```

**Regla de escala CSS:**
- Componentes reutilizables (≥2 páginas) → `common-styles.css` usando variables.
- Estilos específicos de un módulo → `pages/{modulo}.css` cargado vía `@section Styles`.

```cshtml
@section Styles {
    <link href="~/css/pages/propiedades.css" rel="stylesheet" />
}
```

---

## 10. Sistema de Notificaciones (Patrón Pasivo)

En lugar de acoplamiento directo entre módulos, se usa la **base de datos como bus de eventos pasivo**. Cualquier servicio puede emitir notificaciones sin conocer quién las consumirá:

```csharp
// Desde cualquier Service — patrón estándar de emisión:
_context.Notificaciones.Add(new Notificacion
{
    IdUsuarios       = usuarioDestino.Id,
    Titulo           = "Alerta del Sistema",
    Mensaje          = "Operación ejecutada correctamente.",
    Tipo             = "Success",  // "Info" | "Warning" | "Danger"
    RutaRedireccion  = $"/Modulo/Accion/{id}"
});
await _context.SaveChangesAsync();
```

El `NotificacionNavbarFilter` intercepta cada request de Razor Pages, consulta el conteo de notificaciones no leídas del usuario autenticado y lo inyecta en `ViewData["NotifCount"]`. El `_Layout.cshtml` lo renderiza en el navbar de forma transparente — ningún `PageModel` individual necesita ocuparse de esto.

---

## 11. Autenticación y Seguridad

| Mecanismo | Detalle |
|---|---|
| Auth local | Cookie `MySmartDeviceCookie` · Expiración: 30 min |
| OAuth2 Google | Flujo con cookie temporal `ExternalCookie` + callback `/signin-google` |
| Roles | Sistema de roles persistido en BD (`Roles.cs`) |
| Acceso denegado | Redirect a `/Denegada` |
| Sanitización | `ISoporteService.SanitizarTexto()` — strip HTML + control chars + truncado |
| MQTT Auth | Credenciales broker vía `IConfiguration` (nunca hardcodeadas) |
| API Commands | `[IgnoreAntiforgeryToken]` en `MqttComandosController` (endpoint REST interno) |

---

## 12. Decisión Arquitectónica — Service Pattern vs. Generic Repository

**Pregunta:** ¿Por qué no se usa un `IRepository<T>` genérico sobre EF Core?

| Eje | Generic Repository | Service Pattern Específico |
|---|---|---|
| **Rendimiento** | Idéntico (EF Core usa DbSets internamente) | Idéntico |
| **Mantenibilidad** | Anti-patrón conocido: capa de abstracción vacía sobre una abstracción existente (`DbContext` ya es Repository + Unit of Work) | Alta: cada servicio encapsula la lógica de negocio propia del dominio |
| **Complejidad** | Alta (genéricos, especificaciones, etc.) sin beneficio real | Baja: cada `XxxService` tiene métodos semánticos y lógica de validación propia |

**Dictamen Tech Lead:** Se usa **Service Pattern Específico**. `AppDbContext` se inyecta únicamente en los `Services`, no en `PageModels` ni `Controllers`. No se implementan repositorios genéricos sobre EF Core.

---

## 13. Índice de Documentación Técnica

| Archivo | Contenido |
|---|---|
| [`docs/ARCHITECTURE.md`](./ARCHITECTURE.md) | Este documento — arquitectura global |
| [`docs/architecture/modulo_inmuebles.md`](./architecture/modulo_inmuebles.md) | Blueprint: Propiedades, Espacios, Ubicaciones |
| [`docs/architecture/modulo_dispositivos.md`](./architecture/modulo_dispositivos.md) | Blueprint: Dispositivos, Soportes |
| [`docs/architecture/modulo_administrativo.md`](./architecture/modulo_administrativo.md) | Blueprint: Dashboard, Configuraciones |
| [`docs/architecture/modulo_perfil_notificaciones.md`](./architecture/modulo_perfil_notificaciones.md) | Blueprint: Perfil, Notificaciones |
| [`docs/architecture/clean_architecture_perfil.md`](./architecture/clean_architecture_perfil.md) | Diagnóstico Clean Architecture — Perfil |
| [`docs/MQTT/Arquitectura_Bidireccional_IoT.md`](./MQTT/Arquitectura_Bidireccional_IoT.md) | Arquitectura MQTT + SignalR bidireccional |
| [`docs/Usuarios/Usuarios_CRUD_Blueprint.md`](./Usuarios/Usuarios_CRUD_Blueprint.md) | Documentación CRUD módulo Usuarios |
| [`docs/Propiedades/PropiedadService_doc.md`](./Propiedades/PropiedadService_doc.md) | Documentación servicio Propiedades |

---

_Documentación generada y avalada por el Arquitecto de Software (arch-1). Toda modificación estructural debe reflejarse en este documento antes de ser entregada al agente de ejecución (dev-1)._
