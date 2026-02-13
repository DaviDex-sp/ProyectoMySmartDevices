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
    public class DetailsModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public DetailsModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        public Espacio Espacio { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var espacio = await _context.Espacios.FirstOrDefaultAsync(m => m.Id == id);

            if (espacio is not null)
            {
                Espacio = espacio;

                return Page();
            }

            return NotFound();
        }
    }
}
