using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Dispositivos
{
    [Authorize(Roles = "Admin, Usuario, Propietario")]
    public class CreateModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public CreateModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Cargamos los espacios para el Select (Dropdown)
            ViewData["IdEspacio"] = new SelectList(await _context.Espacios.ToListAsync(), "Id", "Nombre");
            return Page();
        }

        [BindProperty]
        public Dispositivo Dispositivo { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Si falla la validación, debemos recargar la lista de espacios
                ViewData["IdEspacio"] = new SelectList(await _context.Espacios.ToListAsync(), "Id", "Nombre");
                return Page();
            }

            _context.Dispositivos.Add(Dispositivo);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}