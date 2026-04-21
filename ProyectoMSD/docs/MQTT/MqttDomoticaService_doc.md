# Documentación: MqttDomoticaService

## Propósito
Este servicio se ejecuta en segundo plano (`BackgroundService`) dentro del pipeline de ASP.NET Core y está diseñado para establecer y mantener una conexión MQTT persistente con el broker en la nube (HiveMQ). Su objetivo principal es escuchar en tiempo real las publicaciones de datos (telemetría, clima, control) emitidas por los dispositivos IoT (ESP32 / Wokwi) y eventualmente propagar estos mensajes hacia las capas superiores de "MySmartDevice".

## Dependencias
- **Librería MqttNet (4.x):** Paquete `MQTTnet` para implementar el cliente MQTT y las suscripciones. 
- **Servicios Inyectados:** `ILogger<MqttDomoticaService>` para el rastreo y auditoría en la consola.
- **Registro en el Ciclo de Vida:** Añadido mediante `AddHostedService<MqttDomoticaService>()` en `Program.cs`.
- **HiveMQ Cloud TLS:** Requiere el uso de TLS para conectarse de forma segura usando las opciones `.WithTlsOptions()`.

## Especificaciones de Conexión (API Specs)
- **Broker TCP:** `c48736d53e424d229db6884844c54666.s1.eu.hivemq.cloud`
- **Puerto:** `8883` (Conexión Segura MQTTS)
- **Credenciales:** `MillerHiveMQ` / `Yaramiller35` *(Nota del Arquitecto: Evaluar migración de estas credenciales hacia Azure KeyVault / `IConfiguration` en fases futuras).*
- **Suscripciones Activas:** 
  - `domotica/sensores/clima`

## Resiliencia y Fallos
Si el broker se desconecta abruptamente, se captura la excepción a través del evento `DisconnectedAsync`, el cual instanciará un bucle de reintento (`Task.Delay(TimeSpan.FromSeconds(5))`) de manera infinita hasta recuperar la conectividad de red o la disponibilidad del servicio de HiveMQ.

## Flujo de Trabajo y Sincronismo DOM (SignalR)
Durante el ciclo inicial del proyecto, los datos de hardware eran limitados a visualización en consola. Actualmente se rediseñó la arquitectura de propagación:

Al llegar un payload en el evento `ApplicationMessageReceivedAsync`, el servicio inyecta un `IHubContext<DispositivoHub>` de ASP.NET SignalR de forma perimetral. Utiliza el conector `Clients.All.SendAsync("ReceiveTelemetry", topic, payload)` para reenviar el binario codificado directamente al Socket TLS / HTTPS abierto en el navegador de los usuarios con rol de administrador en la Vista `Details.cshtml`.
Esto facilita una arquitectura *Pushing/Reactive* en vez de un Polling Ajax pesado, liberando de consultas innecesarias al sistema de Base de Datos principal (MySQL).
