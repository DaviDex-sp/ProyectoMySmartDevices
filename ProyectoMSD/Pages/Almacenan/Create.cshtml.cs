using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Almacenan
{
    public class CreateModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public CreateModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["IdDispositivos"] = new SelectList(_context.Dispositivos, "Id", "Id");
        ViewData["IdEspacios"] = new SelectList(_context.Espacios, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public ProyectoMSD.Modelos.Almacenan Almacenan { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Almacenans.Add(Almacenan);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
