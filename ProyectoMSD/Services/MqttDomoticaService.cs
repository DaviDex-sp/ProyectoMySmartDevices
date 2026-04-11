using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace ProyectoMSD.Services
{
    public class MqttDomoticaService : BackgroundService
    {
        private readonly IMqttClient _mqttClient;
        private readonly ILogger<MqttDomoticaService> _logger;

        public MqttDomoticaService(ILogger<MqttDomoticaService> logger)
        {
            _logger = logger;
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 1. Configurar opciones de conexión a HiveMQ Cloud
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("c48736d53e424d229db6884844c54666.s1.eu.hivemq.cloud", 8883)
                .WithCredentials("MillerHiveMQ", "Yaramiller35")
                .WithTlsOptions(o => o.UseTls()) // Obligatorio para HiveMQ
                .WithClientId("CSharpBackend_" + Guid.NewGuid().ToString())
                .Build();

            // 2. Configurar qué hacer cuando se desconecta
            _mqttClient.DisconnectedAsync += async e =>
            {
                _logger.LogWarning("Desconectado de HiveMQ. Reintentando en 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                try
                {
                    await _mqttClient.ConnectAsync(options, stoppingToken);
                }
                catch { }
            };

            // 3. Configurar qué hacer cuando llega un mensaje
            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

                // Por ahora, lo imprimimos en la consola de Visual Studio.
                // En el siguiente paso, enviaremos esto a tu Razor Page.
                _logger.LogInformation($"[MQTT RECIBIDO] Tópico: {topic} | Payload: {payload}");

                return Task.CompletedTask;
            };

            // 4. Conectar y suscribirse
            try
            {
                _logger.LogInformation("Conectando a HiveMQ...");
                await _mqttClient.ConnectAsync(options, stoppingToken);
                _logger.LogInformation("¡Conectado exitosamente a HiveMQ desde C#!");

                // Nos suscribimos al mismo tópico donde publica el ESP32
                var subscribeOptions = new MqttFactory().CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f.WithTopic("domotica/sensores/clima"))
                    .Build();

                await _mqttClient.SubscribeAsync(subscribeOptions, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al conectar con MQTT: {ex.Message}");
            }
        }
    }
}
