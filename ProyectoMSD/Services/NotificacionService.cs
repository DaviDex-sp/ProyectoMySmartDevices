using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Services
{
    /// <summary>
    /// Implementacion del servicio de notificaciones automaticas.
    /// Responsable de persistir notificaciones hacia usuarios individuales
    /// o grupos segmentados por rol. Ciclo de vida: Scoped.
    /// </summary>
    public class NotificacionService : INotificacionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NotificacionService> _logger;

        public NotificacionService(AppDbContext context, ILogger<NotificacionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task CrearAsync(CrearNotificacionDto dto)
        {
            var notificacion = new Notificacion
            {
                IdUsuarios      = dto.IdUsuario,
                Titulo          = dto.Titulo,
                Mensaje         = dto.Mensaje,
                Tipo            = dto.Tipo,
                Leida           = false,
                FechaCreacion   = DateTime.UtcNow,
                RutaRedireccion = dto.RutaRedireccion
            };

            _context.Notificaciones.Add(notificacion);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation(
                    "[NotificacionService] Notificacion tipo '{Tipo}' creada para usuario ID {IdUsuario}.",
                    dto.Tipo, dto.IdUsuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[NotificacionService] Error al persistir notificacion para usuario ID {IdUsuario}.",
                    dto.IdUsuario);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task CrearParaRolAsync(string rol, string titulo, string mensaje, string tipo, string? ruta = null)
        {
            var idsDestinatarios = await _context.Usuarios
                .Where(u => u.Rol == rol)
                .Select(u => u.Id)
                .ToListAsync();

            if (idsDestinatarios.Count == 0)
            {
                _logger.LogWarning(
                    "[NotificacionService] No se encontraron usuarios con rol '{Rol}' para broadcast.", rol);
                return;
            }

            var ahora = DateTime.UtcNow;
            var notificaciones = idsDestinatarios.Select(uid => new Notificacion
            {
                IdUsuarios      = uid,
                Titulo          = titulo,
                Mensaje         = mensaje,
                Tipo            = tipo,
                Leida           = false,
                FechaCreacion   = ahora,
                RutaRedireccion = ruta
            }).ToList();

            _context.Notificaciones.AddRange(notificaciones);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation(
                    "[NotificacionService] Broadcast tipo '{Tipo}' enviado a {Count} usuario(s) con rol '{Rol}'.",
                    tipo, notificaciones.Count, rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[NotificacionService] Error en broadcast para rol '{Rol}'.", rol);
                throw;
            }
        }
    }
}