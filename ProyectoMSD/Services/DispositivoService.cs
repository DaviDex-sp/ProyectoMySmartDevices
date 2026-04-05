using System.Collections.Generic;
using System.Linq;
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

        public DispositivoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DispositivoDto>> GetDispositivosAsync()
        {
            var dispositivos = await _context.Dispositivos.ToListAsync();
            return dispositivos.Select(d => new DispositivoDto
            {
                Id = d.Id,
                Nombre = d.Nombre ?? string.Empty,
                Estado = d.Estado ?? string.Empty,
                Tipo = d.Tipo ?? string.Empty,
                Marca = d.Marca ?? string.Empty,
                Usos = d.Usos ?? string.Empty,
                IsActive = DispositivoActivo(d.Estado),
                IconClass = ObtenerIcono(d.Tipo),
                BadgeClass = ObtenerBadge(d.Tipo)
            }).ToList();
        }

        public async Task<int> GetTotalDispositivosAsync()
        {
            return await _context.Dispositivos.CountAsync();
        }

        private bool DispositivoActivo(string? estado)
        {
            if (string.IsNullOrEmpty(estado)) return false;
            var e = estado.ToLower().Trim();
            return e == "activo" || e == "encendido" || e == "on" || e == "1" || e == "habilitado" || e == "conectado" || e == "funcionando";
        }

        private string ObtenerIcono(string? tipo)
        {
            if (string.IsNullOrEmpty(tipo)) return "fas fa-microchip";
            var t = tipo.ToLower();
            if (t.Contains("camara") || t.Contains("cámara")) return "fas fa-camera";
            if (t.Contains("luz")) return "fas fa-lightbulb";
            if (t.Contains("puerta")) return "fas fa-door-open";
            return "fas fa-microchip";
        }

        private string ObtenerBadge(string? tipo)
        {
            if (string.IsNullOrEmpty(tipo)) return "bg-light text-dark";
            var t = tipo.ToLower();
            if (t.Contains("camara") || t.Contains("cámara")) return "badge-camara";
            if (t.Contains("luz")) return "badge-luz";
            if (t.Contains("puerta")) return "badge-puerta";
            return "bg-light text-dark";
        }

        public async Task<bool> ToggleEstadoAsync(int id)
        {
            var dispositivo = await _context.Dispositivos.FindAsync(id);
            if (dispositivo == null) return false;

            if (DispositivoActivo(dispositivo.Estado))
            {
                dispositivo.Estado = "Inactivo";
            }
            else
            {
                dispositivo.Estado = "Activo";
            }

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
    }
}
