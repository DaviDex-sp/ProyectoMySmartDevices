using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Services
{
    public class EspacioService : IEspacioService
    {
        private readonly AppDbContext _context;

        public EspacioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EspacioDto>> GetEspaciosConDispositivosAsync()
        {
            var espacios = await _context.Espacios
                .Include(e => e.IdPropiedadesNavigation)
                .Include(e => e.Dispositivos)
                .ToListAsync();

            return espacios.Select(e => new EspacioDto
            {
                Id = e.Id,
                Nombre = e.Nombre ?? string.Empty,
                IdPropiedades = e.IdPropiedades,
                NombrePropiedad = e.IdPropiedadesNavigation?.Direccion ?? $"ID: {e.IdPropiedades}",
                TotalDispositivos = e.Dispositivos.Count,
                DispositivosActivos = e.Dispositivos.Count(d => DispositivoActivo(d.Estado)),
                DispositivosInactivos = e.Dispositivos.Count(d => !DispositivoActivo(d.Estado)),
                Dispositivos = e.Dispositivos.Select(d => new DispositivoDto
                {
                    Id = d.Id,
                    Nombre = d.Nombre ?? string.Empty,
                    Estado = d.Estado ?? string.Empty,
                    Tipo = d.Tipo ?? string.Empty,
                    IsActive = DispositivoActivo(d.Estado),
                    IconClass = ObtenerIcono(d.Tipo),
                    TextColorClass = DispositivoActivo(d.Estado) ? ObtenerColorActivo(d.Tipo) : "text-danger"
                }).ToList()
            }).ToList();
        }

        public async Task<int> GetTotalEspaciosAsync()
        {
            return await _context.Espacios.CountAsync();
        }

        private bool DispositivoActivo(string? estado)
        {
            if (string.IsNullOrEmpty(estado)) return false;
            var e = estado.ToLower().Trim();
            return e == "activo" || e == "encendido" || e == "on" || e == "1" || e == "habilitado" || e == "conectado" || e == "funcionando" || e.Contains("en uso") || e.Contains("trabajando") || e.Contains("reposo");
        }

        private string ObtenerIcono(string? tipo)
        {
            if (string.IsNullOrEmpty(tipo)) return "fas fa-microchip";
            var t = tipo.ToLower();
            if (t.Contains("termostato")) return "fas fa-thermometer-half";
            if (t.Contains("luz")) return "fas fa-lightbulb";
            if (t.Contains("camara") || t.Contains("cámara")) return "fas fa-camera";
            if (t.Contains("sensor")) return "fas fa-eye";
            if (t.Contains("altavoz")) return "fas fa-volume-up";
            if (t.Contains("tv") || t.Contains("television")) return "fas fa-tv";
            if (t.Contains("puerta")) return "fas fa-door-open";
            return "fas fa-microchip";
        }

        private string ObtenerColorActivo(string? tipo)
        {
            if (string.IsNullOrEmpty(tipo)) return "text-secondary";
            var t = tipo.ToLower();
            if (t.Contains("termostato")) return "text-danger";
            if (t.Contains("luz")) return "text-warning";
            if (t.Contains("camara") || t.Contains("cámara")) return "text-info";
            if (t.Contains("sensor")) return "text-primary";
            if (t.Contains("altavoz")) return "text-success";
            if (t.Contains("tv") || t.Contains("television")) return "text-dark";
            return "text-secondary";
        }
    }
}
