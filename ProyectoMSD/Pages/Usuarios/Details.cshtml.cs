using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using ProyectoMSD.Interfaces;

namespace ProyectoMSD.Pages.Usuarios
{
    public class DetailsModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly AppDbContext _context;

        public DetailsModel(IUsuarioService usuarioService, AppDbContext context)
        {
            _usuarioService = usuarioService;
            _context = context;
        }

        public Usuario Usuario { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _usuarioService.GetUsuarioByIdAsync(id.Value);
            
            if (usuario is not null)
            {
                // Cargar explícitamente las propiedades para estar seguros, ya que el servicio puede no traer los Includes nuevos
                Usuario = await _context.Usuarios
                    .Include(u => u.UsuariosPropiedades)
                        .ThenInclude(up => up.IdPropiedadNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id.Value) ?? usuario;

                return Page();
            }

            return NotFound();
        }
    }
}
