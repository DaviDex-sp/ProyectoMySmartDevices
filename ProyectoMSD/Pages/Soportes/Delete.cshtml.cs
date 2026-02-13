using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Soportes
{
    public class DeleteModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public DeleteModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Soporte Soporte { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var soporte = await _context.Soportes.FirstOrDefaultAsync(m => m.Id == id);

            if (soporte is not null)
            {
                Soporte = soporte;

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

            var soporte = await _context.Soportes.FindAsync(id);
            if (soporte != null)
            {
                Soporte = soporte;
                _context.Soportes.Remove(Soporte);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
