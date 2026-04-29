using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Dispositivos
{
    /// <summary>
    /// PageModel para el formulario de creacion de dispositivos IoT.
    /// Orquesta la llamada a IDispositivoService.CreateAsync y propaga
    /// el ID del usuario autenticado para la notificacion automatica.
    /// Sigue el principio de PageModel delgado: sin logica de negocio inline.
    /// </summary>
    [Authorize(Roles = "Admin, Usuario, Propietario")]
    public class CreateModel : PageModel
    {
        private readonly IDispositivoService _dispositivoService;
        private readonly AppDbContext _context;

        public CreateModel(IDispositivoService dispositivoService, AppDbContext context)
        {
            _dispositivoService = dispositivoService;
            _context            = context;
        }

        [BindProperty]
        public Dispositivo Dispositivo { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["IdEspacio"] = new SelectList(
                await _context.Espacios.AsNoTracking().ToListAsync(), "Id", "Nombre");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["IdEspacio"] = new SelectList(
                    await _context.Espacios.AsNoTracking().ToListAsync(), "Id", "Nombre");
                return Page();
            }

            var userIdClaim = User.FindFirstValue("UserId");
            int idUsuario   = int.TryParse(userIdClaim, out int uid) ? uid : 0;

            await _dispositivoService.CreateAsync(Dispositivo, idUsuario);
            return RedirectToPage("./Index");
        }
    }
}