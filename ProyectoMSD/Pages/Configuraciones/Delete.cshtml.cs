using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Configuraciones
{
    public class DeleteModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public DeleteModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Configuracione Configuracione { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var configuracione = await _context.Configuraciones.FirstOrDefaultAsync(m => m.Codigo == id);

            if (configuracione is not null)
            {
                Configuracione = configuracione;

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

            var configuracione = await _context.Configuraciones.FindAsync(id);
            if (configuracione != null)
            {
                Configuracione = configuracione;
                _context.Configuraciones.Remove(Configuracione);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
