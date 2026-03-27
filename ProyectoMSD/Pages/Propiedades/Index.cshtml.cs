using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Propiedades
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public IndexModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }
        public IList<Usuario> Usuario { get; set; } = default!;
        public IList<Espacio> Espacios { get; set; } = default!;  // ← FALTA ESTO
        public IList<Dispositivo> Dispositivos { get; set; } = default!;

        // Propiedades calculadas para las estadísticas
        public int TotalUsuarios => Usuario?.Count ?? 0;
        public int TotalEspacios => Espacios?.Count ?? 0;
        public int TotalDispositivos => Dispositivos?.Count ?? 0;
        public IList<Propiedade> Propiedade { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var isAdmin = User.IsInRole("Admin");
            IQueryable<Propiedade> propQuery = _context.Propiedades
                .Include(p => p.UsuariosPropiedades)
                    .ThenInclude(up => up.IdUsuarioNavigation)
                .Include(p => p.Espacios); // Added Include just in case it's used in view item.Espacios

            if (!isAdmin)
            {
                var userIdString = User.FindFirst("UserId")?.Value;
                if (int.TryParse(userIdString, out int currentUserId))
                {
                    propQuery = propQuery.Where(p => p.UsuariosPropiedades.Any(up => up.IdUsuario == currentUserId));
                }
                else
                {
                    propQuery = propQuery.Where(p => false);
                }
            }

            Propiedade = await propQuery.ToListAsync();

            // Cargar TODOS los datos que necesitas
            Usuario = await _context.Usuarios.ToListAsync();

            // Si tienes modelo Espacio
            Espacios = await _context.Espacios
                .ToListAsync();

            // Si tienes modelo Dispositivo
            Dispositivos = await _context.Dispositivos
                .ToListAsync();
        }
    }
}
