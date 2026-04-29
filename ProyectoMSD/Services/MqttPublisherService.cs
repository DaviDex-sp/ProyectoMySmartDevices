using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using ProyectoMSD.Interfaces;

namespace ProyectoMSD.Services
{
    /// <summary>
    /// Servicio Singleton responsable de publicar comandos al broker HiveMQ Cloud.
    /// Las credenciales y topicos se leen desde IConfiguration (appsettings.json / variables de entorno).
    /// </summary>
    public class MqttPublisherService : IMqttPublisherService
    {
        private readonly IMqttClient _mqttClient;
        private readonly ILogger<MqttPublisherService> _logger;
        private readonly MqttClientOptions _options;
        private readonly IConfiguration _configuration;

        public MqttPublisherService(ILogger<MqttPublisherService> logger, IConfiguration configuration)
        {
            _logger        = logger;
            _configuration = configuration;

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            var host     = _configuration["Mqtt:Host"]     ?? "c48736d53e424d229db6884844c54666.s1.eu.hivemq.cloud";
            var port     = int.Parse(_configuration["Mqtt:Port"] ?? "8883");
            var username = _configuration["Mqtt:Username"] ?? string.Empty;
            var password = _configuration["Mqtt:Password"] ?? string.Empty;

            _options = new MqttClientOptionsBuilder()
                .WithTcpServer(host, port)
                .WithCredentials(username, password)
                .WithTlsOptions(o => o.UseTls())
                .WithClientId("CSharpPublisher_" + Guid.NewGuid())
                .Build();
        }

        private string TopicComandos =>
            _configuration["Mqtt:TopicComandos"] ?? "domotica/comandos";

        /// <inheritdoc />
        public async Task<bool> PublishCommandAsync(string topic, string payload)
        {
            try
            {
                if (!_mqttClient.IsConnected)
                    await _mqttClient.ConnectAsync(_options);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(Encoding.UTF8.GetBytes(payload))
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                var result = await _mqttClient.PublishAsync(message);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MqttPublisher] Error publicando en topico {Topic}", topic);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> PublishStructuredCommandAsync(string macDestino, string componente, string comando)
        {
            var payload = JsonSerializer.Serialize(new
            {
                mac_destino = macDestino,
                componente  = componente,
                comando     = comando
            });

            _logger.LogInformation(
                "[MqttPublisher] Despachando comando estructurado: {Payload}", payload);

            return await PublishCommandAsync(TopicComandos, payload);
        }
    }
}
