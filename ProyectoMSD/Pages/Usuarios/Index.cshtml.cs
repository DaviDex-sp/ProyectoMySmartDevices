
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Usuarios
{
    [Authorize(Roles = "Admin,Usuario,Huesped")]
    public class IndexModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public IndexModel(ProyectoMSD.Modelos.AppDbContext context) 
        {
            _context = context;
        }

        public IList<Usuario> Usuario { get;set; } = default!;
        public IList<Espacio> Espacios { get; set; } = default!;  // ← FALTA ESTO
        public IList<Dispositivo> Dispositivos { get; set; } = default!;

        // Propiedades calculadas para las estadísticas
        public int TotalUsuarios => Usuario?.Count ?? 0;
        public int TotalEspacios => Espacios?.Count ?? 0;
        public int TotalDispositivos => Dispositivos?.Count ?? 0;
        public int UsuariosActivos => Usuario?.Count() ?? 0;
        public int DispositivosActivos => Dispositivos?.Count() ?? 0;
        

        public async Task OnGetAsync()
        {
            // Cargar TODOS los datos que necesitas
            Usuario = await _context.Usuarios.ToListAsync();

            // Si tienes modelo Espacio
            Espacios = await _context.Espacios  // ← Incluye dispositivos relacionados
                .ToListAsync();

            // Si tienes modelo Dispositivo
            Dispositivos = await _context.Dispositivos
                .ToListAsync();

          
        }
    }
}

