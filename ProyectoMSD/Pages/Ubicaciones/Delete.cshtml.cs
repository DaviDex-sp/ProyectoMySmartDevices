using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Ubicaciones
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ubicacione Ubicacione { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ubicacione = await _context.Ubicaciones.FirstOrDefaultAsync(m => m.Id == id);

            if (ubicacione == null)
            {
                return NotFound();
            }
            else 
            {
                Ubicacione = ubicacione;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ubicacione = await _context.Ubicaciones.FindAsync(id);

            if (ubicacione != null)
            {
                Ubicacione = ubicacione;
                _context.Ubicaciones.Remove(Ubicacione);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
