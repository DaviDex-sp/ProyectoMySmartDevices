using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Dispositivos
{
    public class IndexModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public IndexModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        public IList<Dispositivo> Dispositivo { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Dispositivo = await _context.Dispositivos.ToListAsync();
        }

        // MÉTODO QUE USA TUS ESTADOS REALES, NO MIS SUPOSICIONES
        public async Task<IActionResult> OnPostToggleEstadoAsync(int id)
        {
            var dispositivo = await _context.Dispositivos.FindAsync(id);
            if (dispositivo == null)
            {
                return NotFound();
            }

            // AQUÍ NECESITO SABER QUÉ ESTADOS ESPECÍFICOS TIENES
            // ¿Cuáles son TUS valores exactos de Estado?
            // Ejemplo genérico basado en estados comunes:

            switch (dispositivo.Estado?.ToLower())
            {
                case "activo":
                    dispositivo.Estado = "Inactivo";
                    break;
                case "inactivo":
                    dispositivo.Estado = "Activo";
                    break;
                case "conectado":
                    dispositivo.Estado = "Desconectado";
                    break;
                case "desconectado":
                    dispositivo.Estado = "Conectado";
                    break;
                case "encendido":
                    dispositivo.Estado = "Apagado";
                    break;
                case "apagado":
                    dispositivo.Estado = "Encendido";
                    break;
                case "online":
                    dispositivo.Estado = "Offline";
                    break;
                case "offline":
                    dispositivo.Estado = "Online";
                    break;
                default:
                    // Si no reconoce el estado, no hacer nada
                    return RedirectToPage();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Manejar error de base de datos
                return BadRequest();
            }

            return RedirectToPage();
        }
    }
}


