using MQTTnet;
using MQTTnet.Client;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ProyectoMSD.Hubs;
using ProyectoMSD.Interfaces;

namespace ProyectoMSD.Services
{
    /// <summary>
    /// Servicio en segundo plano (Singleton / BackgroundService) que mantiene
    /// la conexion activa con el broker HiveMQ Cloud y retransmite telemetria
    /// MQTT al frontend via SignalR. Genera notificaciones automaticas la
    /// primera vez que cada topico envia datos al sistema.
    /// </summary>
    public class MqttDomoticaService : BackgroundService
    {
        private readonly IMqttClient _mqttClient;
        private readonly ILogger<MqttDomoticaService> _logger;
        private readonly IHubContext<DispositivoHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Conjunto de topicos que ya emitieron su notificacion de primer dato.
        /// Garantiza que el broadcast se emita exactamente una vez por topico
        /// durante el ciclo de vida del servicio.
        /// </summary>
        private readonly HashSet<string> _topicsNotificados = new();

        public MqttDomoticaService(
            ILogger<MqttDomoticaService> logger,
            IHubContext<DispositivoHub> hubContext,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _logger        = logger;
            _hubContext    = hubContext;
            _scopeFactory  = scopeFactory;
            _configuration = configuration;
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 1. Leer credenciales desde IConfiguration (nunca hardcodeadas)
            var host     = _configuration["Mqtt:Host"]     ?? "c48736d53e424d229db6884844c54666.s1.eu.hivemq.cloud";
            var port     = int.Parse(_configuration["Mqtt:Port"] ?? "8883");
            var username = _configuration["Mqtt:Username"] ?? string.Empty;
            var password = _configuration["Mqtt:Password"] ?? string.Empty;
            var topic    = _configuration["Mqtt:Topic"]    ?? "domotica/sensores/clima";

            // 2. Construir opciones de conexion
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(host, port)
                .WithCredentials(username, password)
                .WithTlsOptions(o => o.UseTls())
                .WithClientId("CSharpBackend_" + Guid.NewGuid())
                .Build();

            // 3. Reconexion automatica ante desconexion
            _mqttClient.DisconnectedAsync += async e =>
            {
                _logger.LogWarning("[MQTT] Desconectado de HiveMQ. Reintentando en 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                try { await _mqttClient.ConnectAsync(options, stoppingToken); }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[MQTT] Fallo al reconectar con el broker.");
                }
            };

            // 4. Handler de mensajes entrantes
            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var receivedTopic = e.ApplicationMessage.Topic;
                var payload       = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

                _logger.LogInformation(
                    "[MQTT] Topico: {Topic} | Payload: {Payload}", receivedTopic, payload);

                // Retransmitir telemetria al frontend via SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveTelemetry", receivedTopic, payload);

                // Notificar solo la PRIMERA vez que este topico transmite datos
                if (_topicsNotificados.Add(receivedTopic))
                {
                    await EmitirNotificacionTelemetriaAsync(receivedTopic);
                }
            };

            // 5. Conectar y suscribirse al topico configurado
            try
            {
                _logger.LogInformation("[MQTT] Conectando a HiveMQ en {Host}:{Port}...", host, port);
                await _mqttClient.ConnectAsync(options, stoppingToken);
                _logger.LogInformation("[MQTT] Conexion establecida exitosamente.");

                var subscribeOptions = new MqttFactory().CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic(topic))
                    .Build();

                await _mqttClient.SubscribeAsync(subscribeOptions, stoppingToken);
                _logger.LogInformation("[MQTT] Suscrito al topico: {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MQTT] Error critico al conectar con el broker.");
            }
        }

        /// <summary>
        /// Crea un scope de DI para acceder a INotificacionService (Scoped)
        /// desde este Singleton y emite el broadcast a todos los Admins.
        /// </summary>
        private async Task EmitirNotificacionTelemetriaAsync(string topicRecibido)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var notifService = scope.ServiceProvider.GetRequiredService<INotificacionService>();

                await notifService.CrearParaRolAsync(
                    rol:     "Admin",
                    titulo:  "Dispositivo Enviando Datos",
                    mensaje: $"El sensor en el topico \"{topicRecibido}\" ha comenzado a transmitir telemetria activamente al sistema.",
                    tipo:    "telemetria",
                    ruta:    "/Dispositivos"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[MQTT] Error al generar notificacion de telemetria para topico {Topic}.", topicRecibido);
            }
        }
    }
}