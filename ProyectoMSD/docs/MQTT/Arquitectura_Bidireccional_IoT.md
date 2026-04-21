# Arquitectura Bidireccional IoT (MQTT + SignalR)

## 1. Visión General y Objetivo Arquitectónico
Este documento especifica la infraestructura cloud-native desarrollada para permitir no solo la lectura, sino el **Control Bi-direccional (Lectura y Escritura)** de los sensores IoT emparejados a nuestro ecosistema mediante el Broker HiveMQ.
Con el propósito de erradicar el *Smart-UI Anti-Pattern*, se prohíbe realizar llamadas o conexiones directas de los clientes web TCP al Broker. El backend orquesta toda la seguridad de la tubería, exponiendo API REST y WebSockets seguros al Front-end.

## 2. Mapa de Integración y Componentes (Clean Architecture)

### A. Capa de Presentación Inteligente (SignalR + Fetch API)
- **`Pages/Dispositivos/Details.cshtml`**: La interfaz aplica *Glassmorphism* y divide el componente en Paneles (`.prop-detail-panel`). Se inyecta `signalR.min.js` para capturar la telemetría y actualizar medidores mediante manipulaciones directas sobre el DOM, logrando el Tiempo Real sin recargar. Las pulsaciones de botones ejecutan Promesas asíncronas de javascript (`fetch()`) dirigidas a nuestra propia REST API, sin exponer credenciales MQTT al browser.

### B. Capa API REST de Comandancia (C# Controller)
- **`Controllers/Api/MqttComandosController.cs`**: Endpoint estructurado `[ApiController]` y decorado con `[IgnoreAntiforgeryToken]` para recibir tramas POST en el endpoint predefinido (`/api/MqttComandos/enviar`). Deserializa el JSON del cliente en un `ComandoDispositivoDto` verificando nulidades. Instancia y delega al servicio publicador la emisión final al broker IoT.

### C. Contratos Inyectables e Inversión de Dependencias (DIP)
- **`Interfaces/IMqttPublisherService.cs`**: Interfaz unificada con la firma `Task<bool> PublishCommandAsync(string topic, string payload)`. Garantiza que el `MqttComandosController` no dependa del cliente MQTTNet directamente. Si el día de mañana se cambia HiveMQ por Azure IoT Hub, la controladora queda inalterada.
- **`Modelos/DTOs/ComandoDispositivoDto.cs`**: Clase sellada que filtra los datos enviados por la red. (`TargetTopic`, `Accion`, `Valor`).

### D. Capa de Publicación (Singleton Service)
- **`Services/MqttPublisherService.cs`**: Al contrario del `MqttDomoticaService` (que es un `BackgroundService`), la tubería de bajada hacia el dispostivo es manejada por este servicio Singleton puro. Mantiene una sesión persistente (no bloqueante) en HiveMQ exclusivamente para el envío de instrucciones (`Task.Run` > `.PublishAsync()`).

### E. Canal de Multiplexación de Subida (WebSockets)
- **`Hubs/DispositivoHub.cs`**: Hereda de `Hub`, registrándose en tiempo de ejecución en `Program.cs`. Permite ser inyectado como `IHubContext` en el BackgroundService, funcionando como el megáfono principal que grita los datos del payload Mqtt al navegador.

## 3. Directivas de Escalabilidad y Casos de Fallo Requeridos (Resilience Polices)
- **Conexiones Concurrentes MQTT**: Se configuran identidades (`ClientId`) dinámicas adjuntando `Guid.NewGuid()` asegurando que al escalar el App Service horizontalmente en contenedores Docker, los clientes no emitan colisiones destructivas ni sufran caídas en ciclo ("Ping/Pong Kicking").
- **Tolerancia a Caídas y Reintentos**: Ante intermitencia de SignalR, los clientes JS invocan `.withAutomaticReconnect()`. En el backend, de caerse la conexión con HiveMQ `MqttPublisherService` ejecutará reconexión transaccional sin interrumpir los procesos nativos IIS/Kestrel.

---
_Documentación Avalada y Generada por el Arquitecto de Software._
