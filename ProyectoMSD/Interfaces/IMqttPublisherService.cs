using System.Threading.Tasks;

namespace ProyectoMSD.Interfaces
{
    public interface IMqttPublisherService
    {
        /// <summary>
        /// Publica un mensaje de forma asíncrona hacia el Broker HiveMQ
        /// </summary>
        Task<bool> PublishCommandAsync(string topic, string payload);
    }
}
