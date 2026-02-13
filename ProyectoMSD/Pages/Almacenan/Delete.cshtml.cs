using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Almacenan
{
    public class DeleteModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public DeleteModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProyectoMSD.Modelos.Almacenan Almacenan { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var almacenan = await _context.Almacenans.FirstOrDefaultAsync(m => m.Id == id);

            if (almacenan is not null)
            {
                Almacenan = almacenan;

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

            var almacenan = await _context.Almacenans.FindAsync(id);
            if (almacenan != null)
            {
                Almacenan = almacenan;
                _context.Almacenans.Remove(Almacenan);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
