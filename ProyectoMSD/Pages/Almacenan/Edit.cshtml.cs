using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Almacenan
{
    public class EditModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public EditModel(ProyectoMSD.Modelos.AppDbContext context)
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

            var almacenan =  await _context.Almacenans.FirstOrDefaultAsync(m => m.Id == id);
            if (almacenan == null)
            {
                return NotFound();
            }
            Almacenan = almacenan;
           ViewData["IdDispositivos"] = new SelectList(_context.Dispositivos, "Id", "Id");
           ViewData["IdEspacios"] = new SelectList(_context.Espacios, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Almacenan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlmacenanExists(Almacenan.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AlmacenanExists(int id)
        {
            return _context.Almacenans.Any(e => e.Id == id);
        }
    }
}
