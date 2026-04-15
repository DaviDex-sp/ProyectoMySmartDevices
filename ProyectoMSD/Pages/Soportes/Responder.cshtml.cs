using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Soportes
{
    [Authorize(Roles = "Admin")]
    public class ResponderModel : PageModel
    {
        private readonly ISoporteService _soporteService;
        private readonly AppDbContext _context;

        public ResponderModel(ISoporteService soporteService, AppDbContext context)
        {
            _soporteService = soporteService;
            _context = context;
        }

        public Soporte Soporte { get; set; } = default!;

        [BindProperty]
        public string Respuesta { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var soporte = await _soporteService.ObtenerPorIdAsync(id.Value, incluirUsuario: true);
            if (soporte == null)
                return NotFound();

            Soporte = soporte;
            Respuesta = soporte.Respuesta ?? string.Empty;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(Respuesta))
            {
                ModelState.AddModelError(nameof(Respuesta), "La respuesta no puede estar vacía.");
                Soporte = (await _soporteService.ObtenerPorIdAsync(id, incluirUsuario: true))!;
                return Page();
            }

            // El servicio aplica SanitizarTexto antes de persistir
            var respondido = await _soporteService.ResponderAsync(id, Respuesta);
            if (!respondido)
                return NotFound();

            // Recuperar el soporte para obtener el IdUsuarios de la notificación
            var soporte = await _context.Soportes.FindAsync(id);
            if (soporte != null)
            {
                _context.Notificaciones.Add(new Notificacion
                {
                    IdUsuarios      = soporte.IdUsuarios,
                    Titulo          = "Ticket respondido",
                    Mensaje         = $"Tu ticket #{id} ha sido respondido por el equipo de soporte.",
                    Tipo            = "TicketRespondido",
                    Leida           = false,
                    FechaCreacion   = DateTime.Now,
                    RutaRedireccion = $"/Soportes/Details?id={id}"
                });
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Ticket #{id} respondido exitosamente.";
            return RedirectToPage("./Index");
        }
    }
}
