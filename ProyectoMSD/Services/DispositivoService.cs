using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Services
{
    public class DispositivoService : IDispositivoService
    {
        private readonly AppDbContext _context;
        private readonly INotificacionService _notificacionService;

        public DispositivoService(AppDbContext context, INotificacionService notificacionService)
        {
            _context = context;
            _notificacionService = notificacionService;
        }

        public async Task<List<DispositivoDto>> GetDispositivosAsync()
        {
            var dispositivos = await _context.Dispositivos.AsNoTracking().ToListAsync();
            return dispositivos.Select(d => new DispositivoDto
            {
                Id           = d.Id,
                IdEspacio    = d.IdEspacio,
                MAC_Address  = d.MAC_Address ?? string.Empty,
                Protocolo    = d.Protocolo   ?? string.Empty,
                Nombre       = d.Nombre      ?? string.Empty,
                Estado       = d.Estado      ?? string.Empty,
                Tipo         = d.Tipo        ?? string.Empty,
                Marca        = d.Marca       ?? string.Empty,
                Usos         = d.Usos        ?? string.Empty,
                IsActive     = DispositivoActivo(d.Estado),
                IconClass    = ObtenerIcono(d.Tipo),
                BadgeClass   = ObtenerBadge(d.Tipo)
            }).ToList();
        }

        public async Task<int> GetTotalDispositivosAsync()
        {
            return await _context.Dispositivos.CountAsync();
        }

        public async Task<DispositivoDto?> GetByIdAsync(int id)
        {
            var d = await _context.Dispositivos.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id);
            if (d == null) return null;

            return new DispositivoDto
            {
                Id           = d.Id,
                IdEspacio    = d.IdEspacio,
                MAC_Address  = d.MAC_Address ?? string.Empty,
                Protocolo    = d.Protocolo   ?? string.Empty,
                Nombre       = d.Nombre      ?? string.Empty,
                Tipo         = d.Tipo        ?? string.Empty,
                Marca        = d.Marca       ?? string.Empty,
                Usos         = d.Usos        ?? string.Empty,
                Estado       = d.Estado      ?? string.Empty,
                IsActive     = DispositivoActivo(d.Estado),
                IconClass    = ObtenerIcono(d.Tipo),
                BadgeClass   = ObtenerBadge(d.Tipo),
                ComponentesJson = d.ComponentesJson
            };
        }

        public async Task<bool> ToggleEstadoAsync(int id)
        {
            var dispositivo = await _context.Dispositivos.FindAsync(id);
            if (dispositivo == null) return false;

            dispositivo.Estado = DispositivoActivo(dispositivo.Estado) ? "Inactivo" : "Activo";

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(DispositivoDto dto)
        {
            var dispositivo = await _context.Dispositivos.FindAsync(dto.Id);
            if (dispositivo == null) return false;

            dispositivo.IdEspacio   = dto.IdEspacio;
            dispositivo.MAC_Address = dto.MAC_Address;
            dispositivo.Protocolo   = dto.Protocolo;
            dispositivo.Nombre      = dto.Nombre;
            dispositivo.Tipo        = dto.Tipo;
            dispositivo.Marca       = dto.Marca;
            dispositivo.Usos        = dto.Usos;
            dispositivo.Estado      = dto.Estado;
            dispositivo.ComponentesJson = dto.ComponentesJson;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        /// <summary>
        /// Persiste un nuevo Dispositivo y dispara la notificacion de configuracion exitosa.
        /// </summary>
        public async Task<int> CreateAsync(Dispositivo dispositivo, int idUsuarioCreador)
        {
            _context.Dispositivos.Add(dispositivo);
            await _context.SaveChangesAsync();

            await _notificacionService.CrearAsync(new CrearNotificacionDto
            {
                IdUsuario       = idUsuarioCreador,
                Titulo          = "Dispositivo Configurado Exitosamente",
                Mensaje         = $"El dispositivo \"{dispositivo.Nombre}\" (MAC: {dispositivo.MAC_Address}) ha sido registrado correctamente en el sistema.",
                Tipo            = "configuracion",
                RutaRedireccion = $"/Dispositivos/Details?id={dispositivo.Id}"
            });

            return dispositivo.Id;
        }

        /// <summary>
        /// Deserializa ComponentesJson y retorna la lista de componentes controlables del dispositivo.
        /// </summary>
        public async Task<List<ComponenteDto>> GetComponentesAsync(int idDispositivo)
        {
            var d = await _context.Dispositivos.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == idDispositivo);

            if (d == null || string.IsNullOrWhiteSpace(d.ComponentesJson))
                return new List<ComponenteDto>();

            try
            {
                return JsonSerializer.Deserialize<List<ComponenteDto>>(d.ComponentesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<ComponenteDto>();
            }
            catch
            {
                return new List<ComponenteDto>();
            }
        }

        // ---- Helpers de UI privados ----

        private static bool DispositivoActivo(string? estado)
        {
            if (string.IsNullOrEmpty(estado)) return false;
            var e = estado.ToLower().Trim();
            return e == "activo" || e == "encendido" || e == "on" ||
                   e == "1"      || e == "habilitado" || e == "conectado" || e == "funcionando";
        }

        private static string ObtenerIcono(string? tipo)
        {
            if (string.IsNullOrEmpty(tipo)) return "fas fa-microchip";
            var t = tipo.ToLower();
            if (t.Contains("camara") || t.Contains("camara")) return "fas fa-camera";
            if (t.Contains("luz"))   return "fas fa-lightbulb";
            if (t.Contains("puerta")) return "fas fa-door-open";
            return "fas fa-microchip";
        }

        private static string ObtenerBadge(string? tipo)
        {
            if (string.IsNullOrEmpty(tipo)) return "bg-light text-dark";
            var t = tipo.ToLower();
            if (t.Contains("camara") || t.Contains("camara")) return "badge-camara";
            if (t.Contains("luz"))   return "badge-luz";
            if (t.Contains("puerta")) return "badge-puerta";
            return "bg-light text-dark";
        }
    }
}
