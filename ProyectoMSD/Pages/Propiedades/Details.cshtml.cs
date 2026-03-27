using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Propiedades
{
    public class DetailsModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public DetailsModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        public Propiedade Propiedade { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propiedade = await _context.Propiedades
                .Include(p => p.UsuariosPropiedades)
                .ThenInclude(up => up.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (propiedade != null && !User.IsInRole("Admin"))
            {
                var userIdString = User.FindFirst("UserId")?.Value;
                if (int.TryParse(userIdString, out int currentUserId))
                {
                    if (!propiedade.UsuariosPropiedades.Any(up => up.IdUsuario == currentUserId))
                    {
                        return Forbid();
                    }
                }
                else
                {
                    return Forbid();
                }
            }

            if (propiedade is not null)
            {
                Propiedade = propiedade;

                return Page();
            }

            return NotFound();
        }
    }
}
