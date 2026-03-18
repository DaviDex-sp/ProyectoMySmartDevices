using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Ubicaciones
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

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
            
            Ubicacione = ubicacione;
            return Page();
        }
    }
}
