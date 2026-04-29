# MqttDomoticaService — Documentación del Módulo

## Propósito

`MqttDomoticaService` es un `BackgroundService` (ciclo de vida Singleton) que mantiene una conexión persistente y cifrada (TLS) con el broker HiveMQ Cloud. Cumple tres responsabilidades:

1. **Recepción de telemetría entrante**: escucha los mensajes MQTT publicados por dispositivos IoT (ej. sensores ESP32) en los tópicos configurados.
2. **Retransmisión en tiempo real al frontend**: reenvía los payloads recibidos a todos los clientes de navegador conectados mediante SignalR (evento `ReceiveTelemetry`).
3. **Notificación de primer dato por tópico** *(nuevo)*: emite una notificación de tipo `"telemetria"` a todos los usuarios con rol `Admin` la **primera vez** que cada tópico MQTT transmite datos, utilizando `IServiceScopeFactory` para acceder de forma segura al servicio `Scoped` `INotificacionService` desde este `Singleton`.

---

## Dependencias

| Dependencia | Tipo | Ciclo de vida | Propósito |
|---|---|---|---|
| `ILogger<MqttDomoticaService>` | Logging de ASP.NET Core | Singleton | Registro estructurado de eventos y errores |
| `IHubContext<DispositivoHub>` | SignalR | Singleton | Envío de telemetría a clientes del navegador |
| `IServiceScopeFactory` | DI de ASP.NET Core | Singleton | Crear contextos de DI con alcance Scoped dentro del Singleton |
| `IConfiguration` | Configuración de ASP.NET Core | Singleton | Leer credenciales MQTT desde `appsettings.json` o variables de entorno |
| `MQTTnet` | Paquete NuGet | — | Cliente del protocolo MQTT |

**Registro en DI** (`Program.cs`):
```csharp
builder.Services.AddHostedService<MqttDomoticaService>();
```

> `IServiceScopeFactory` e `IConfiguration` son registrados automáticamente por ASP.NET Core — no requieren registro manual.

---

## Configuración (`appsettings.json`)

Todas las credenciales MQTT están externalizadas. Nunca deben estar hardcodeadas en el código.

```json
{
  "Mqtt": {
    "Host":     "<host-hivemq>",
    "Port":     "8883",
    "Username": "<usuario-hivemq>",
    "Password": "<password-hivemq>",
    "Topic":    "domotica/sensores/clima"
  }
}
```

En producción (Azure App Service), estos valores se configuran como **Application Settings** (variables de entorno), que sobreescriben automáticamente el `appsettings.json`.

---

## Flujo de Telemetría

```
Dispositivo IoT -> Broker HiveMQ -> MqttDomoticaService
                                          |
                                   Hub SignalR -> Navegador (ReceiveTelemetry)
                                          |
                               (solo primer mensaje por tópico)
                                          |
                                INotificacionService.CrearParaRolAsync("Admin")
```

---

## Comportamiento Clave

### `_topicsNotificados` (HashSet interno)

Conjunto en memoria que registra los tópicos que ya emitieron su notificación de primer dato. Evita el spam de notificaciones en sensores de alta frecuencia. Se reinicia al reiniciar la aplicación (comportamiento esperado y documentado).

### `EmitirNotificacionTelemetriaAsync(string topicRecibido)` (privado)

Crea un scope de DI mediante `IServiceScopeFactory`, resuelve `INotificacionService` y llama a `CrearParaRolAsync("Admin", ...)`. Todas las excepciones son capturadas y registradas en el log — el flujo de mensajes MQTT **nunca** se interrumpe por un fallo en la notificación.

### Reconexión automática

Ante una desconexión del broker, el servicio espera 5 segundos y reintenta la conexión. Los fallos de reconexión se registran mediante `ILogger`.

---

## Evento SignalR Emitido

| Nombre del Evento | Hub | Parámetros | Descripción |
|---|---|---|---|
| `ReceiveTelemetry` | `DispositivoHub` | `topic: string`, `payload: string` | Mensaje MQTT crudo retransmitido al navegador |

**Ejemplo de consumo en el frontend** (JavaScript):
```javascript
connection.on("ReceiveTelemetry", function(topic, payload) {
    const data = JSON.parse(payload);
    console.log("[" + topic + "]", data);
});
```

---

## Nota Crítica de Arquitectura: Singleton accediendo a Scoped

Este servicio es un `Singleton`. `INotificacionService` es `Scoped`. Inyectar un servicio `Scoped` directamente en el constructor de un `Singleton` provoca una **dependencia cautiva** — excepción fatal en tiempo de ejecución de ASP.NET Core.

**Patrón correcto aplicado en este servicio:**
```csharp
using var scope = _scopeFactory.CreateScope();
var notifService = scope.ServiceProvider.GetRequiredService<INotificacionService>();
await notifService.CrearParaRolAsync(...);
```

Este patrón crea un scope de DI de corta vida por cada emisión de notificación, resuelve una instancia fresca de `INotificacionService` (con su propio `AppDbContext`), lo utiliza y luego elimina el scope de forma limpia.
