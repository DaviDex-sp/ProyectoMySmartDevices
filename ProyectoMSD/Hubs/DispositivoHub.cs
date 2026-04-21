using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ProyectoMSD.Hubs
{
    public class DispositivoHub : Hub
    {
        // En esta etapa inicial, el Hub solo sirve para emitir datos ("broadcast")
        // desde el Backend hacia los clientes conectados. 
        // No necesitamos métodos de subida desde el JS aquí, los comandos usarán el API Controller.
    }
}
