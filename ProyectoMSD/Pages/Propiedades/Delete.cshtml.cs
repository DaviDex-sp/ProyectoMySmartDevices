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
    public class DeleteModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public DeleteModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Propiedade Propiedade { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propiedade = await _context.Propiedades.FirstOrDefaultAsync(m => m.Id == id);

            if (propiedade is not null)
            {
                Propiedade = propiedade;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propiedade = await _context.Propiedades.FindAsync(id);
            if (propiedade != null)
            {
                Propiedade = propiedade;
                _context.Propiedades.Remove(Propiedade);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
