using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoMSD.Modelos;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Ubicaciones
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Ubicacione Ubicacione { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Ubicaciones.Add(Ubicacione);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
