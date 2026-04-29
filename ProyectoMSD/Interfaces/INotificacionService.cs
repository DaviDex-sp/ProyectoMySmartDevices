using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Interfaces
{
    public interface INotificacionService
    {
        Task CrearAsync(CrearNotificacionDto dto);

        Task CrearParaRolAsync(string rol, string titulo, string mensaje, string tipo, string? ruta = null);
    }
}
