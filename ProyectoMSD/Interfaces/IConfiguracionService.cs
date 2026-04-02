using ProyectoMSD.Modelos;

namespace ProyectoMSD.Interfaces;

public interface IConfiguracionService
{
    Task<IList<Configuracione>> ObtenerTodasAsync();
    Task<IList<Configuracione>> ObtenerPorUsuarioAsync(int userId);
    Task<Configuracione?> ObtenerPorIdAsync(int id);
    Task<bool> CrearAsync(Configuracione configuracion);
    Task<bool> ActualizarAsync(Configuracione configuracion);
    Task<bool> EliminarAsync(int id);
    
    // Métricas de Dashboard/Convicción
    Task<int> ObtenerConteoNotificacionesAsync();
    Task<int> ObtenerConteoDispositivosAsync();
}
