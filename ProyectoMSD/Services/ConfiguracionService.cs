using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Services;

public class ConfiguracionService : IConfiguracionService
{
    private readonly AppDbContext _context;

    public ConfiguracionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Configuracione>> ObtenerTodasAsync()
    {
        return await _context.Configuraciones
            .Include(c => c.IdUsuariosNavigation)
            .Include(c => c.IdDispositivosNavigation)
            .ToListAsync();
    }

    public async Task<IList<Configuracione>> ObtenerPorUsuarioAsync(int userId)
    {
        return await _context.Configuraciones
            .Where(c => c.IdUsuarios == userId)
            .Include(c => c.IdDispositivosNavigation)
            .ToListAsync();
    }

    public async Task<Configuracione?> ObtenerPorIdAsync(int id)
    {
        return await _context.Configuraciones
            .Include(c => c.IdUsuariosNavigation)
            .Include(c => c.IdDispositivosNavigation)
            .FirstOrDefaultAsync(c => c.Codigo == id);
    }

    public async Task<bool> CrearAsync(Configuracione configuracion)
    {
        _context.Configuraciones.Add(configuracion);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ActualizarAsync(Configuracione configuracion)
    {
        _context.Attach(configuracion).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var config = await _context.Configuraciones.FindAsync(id);
        if (config == null) return false;
        
        _context.Configuraciones.Remove(config);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<int> ObtenerConteoNotificacionesAsync()
    {
        return await _context.Notificaciones.CountAsync();
    }

    public async Task<int> ObtenerConteoDispositivosAsync()
    {
        return await _context.Dispositivos.CountAsync();
    }
}
