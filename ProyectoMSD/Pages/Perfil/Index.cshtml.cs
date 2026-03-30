using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Perfil
{
    public class IndexModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;

        public IndexModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;

        [BindProperty]
        public string? NuevaClave { get; set; }

        // Campos de Ubicación
        [BindProperty]
        public string? UbicacionLatitud { get; set; }

        [BindProperty]
        public string? UbicacionLongitud { get; set; }

        [BindProperty]
        public string? UbicacionDireccion { get; set; }

        public string? MensajeExito { get; set; }
        public bool MostrarAvisoGoogle { get; set; }

        public async Task<IActionResult> OnGetAsync(bool completarPerfil = false)
        {
            var userId = ObtenerUsuarioId();
            if (userId == null) return RedirectToPage("/Index");

            var usuario = await _usuarioService.GetUsuarioPerfilAsync(userId.Value);

            if (usuario == null) return NotFound();

            Usuario = usuario;
            MostrarAvisoGoogle = completarPerfil;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = ObtenerUsuarioId();
            if (userId == null) return RedirectToPage("/Index");

            bool result = await _usuarioService.UpdatePerfilAsync(
                userId.Value, 
                Usuario, 
                UbicacionLatitud, 
                UbicacionLongitud, 
                UbicacionDireccion, 
                NuevaClave
            );

            if (!result) return NotFound();

            MensajeExito = "Perfil actualizado correctamente.";

            // Recargar datos actualizados para la vista
            Usuario = await _usuarioService.GetUsuarioPerfilAsync(userId.Value);

            return Page();
        }

        private int? ObtenerUsuarioId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            return null;
        }
    }
}
