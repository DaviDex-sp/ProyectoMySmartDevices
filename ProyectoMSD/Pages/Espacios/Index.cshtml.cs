using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Espacios
{
    public class IndexModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public IndexModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        public IList<Espacio> Espacio { get; set; } = default!;
        public int TotalDispositivos { get; set; }
        public Dictionary<int, List<Dispositivo>> DispositivosPorEspacio { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Cargar espacios con propiedades y dispositivos
            Espacio = await _context.Espacios
                .Include(e => e.IdPropiedadesNavigation)
                .Include(e => e.Almacenans)
                    .ThenInclude(a => a.IdDispositivosNavigation)
                .ToListAsync();

            // Total de dispositivos en el sistema
            TotalDispositivos = await _context.Dispositivos.CountAsync();

            // Crear diccionario de dispositivos por espacio
            foreach (var espacio in Espacio)
            {
                var dispositivos = espacio.Almacenans
                    .Where(a => a.IdDispositivosNavigation != null)
                    .Select(a => a.IdDispositivosNavigation!)
                    .ToList();

                DispositivosPorEspacio[espacio.Id] = dispositivos;
            }
        }

        // Métodos auxiliares
        public List<Dispositivo> GetDispositivos(int espacioId)
        {
            return DispositivosPorEspacio.ContainsKey(espacioId)
                ? DispositivosPorEspacio[espacioId]
                : new List<Dispositivo>();
        }

        public int GetDispositivosActivos(int espacioId)
        {
            var dispositivos = GetDispositivos(espacioId);
            return dispositivos.Count(d =>
                d.Estado.ToLower().Contains("activo") ||
                d.Estado.ToLower().Contains("encendido") ||
                d.Estado.ToLower().Contains("on"));
        }

        public List<Dispositivo> GetTodosLosDispositivos()
        {
            return _context.Dispositivos.ToList();
        }

        public int GetDispositivosInactivos(int espacioId)
        {
            var dispositivos = GetDispositivos(espacioId);
            return dispositivos.Count(d =>
                d.Estado.ToLower().Contains("inactivo") ||
                d.Estado.ToLower().Contains("apagado") ||
                d.Estado.ToLower().Contains("off"));
        }

        public int GetTotalDispositivosEspacio(int espacioId)
        {
            return GetDispositivos(espacioId).Count;
        }
    }
}

