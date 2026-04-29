using System.Threading.Tasks;

namespace ProyectoMSD.Interfaces
{
    public interface IMqttPublisherService
    {
        /// <summary>
        /// Publica un payload string crudo en el topico especificado del broker HiveMQ.
        /// </summary>
        Task<bool> PublishCommandAsync(string topic, string payload);

        /// <summary>
        /// Construye un JSON estructurado { mac_destino, componente, comando }
        /// y lo publica en el topico de comandos configurado en appsettings.json (Mqtt:TopicComandos).
        /// </summary>
        Task<bool> PublishStructuredCommandAsync(string macDestino, string componente, string comando);
    }
}
