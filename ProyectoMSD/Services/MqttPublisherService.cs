using MQTTnet;
using MQTTnet.Client;
using System.Text;
using ProyectoMSD.Interfaces;

namespace ProyectoMSD.Services
{
    public class MqttPublisherService : IMqttPublisherService
    {
        private readonly IMqttClient _mqttClient;
        private readonly ILogger<MqttPublisherService> _logger;
        private readonly MqttClientOptions _options;

        public MqttPublisherService(ILogger<MqttPublisherService> logger)
        {
            _logger = logger;
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            _options = new MqttClientOptionsBuilder()
                .WithTcpServer("c48736d53e424d229db6884844c54666.s1.eu.hivemq.cloud", 8883)
                .WithCredentials("MillerHiveMQ", "Yaramiller35")
                .WithTlsOptions(o => o.UseTls())
                .WithClientId("CSharpPublisher_" + Guid.NewGuid().ToString())
                .Build();
        }

        public async Task<bool> PublishCommandAsync(string topic, string payload)
        {
            try
            {
                if (!_mqttClient.IsConnected)
                {
                    await _mqttClient.ConnectAsync(_options);
                }

                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                var result = await _mqttClient.PublishAsync(applicationMessage);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publicando comando MQTT a {Topic}", topic);
                return false;
            }
        }
    }
}
