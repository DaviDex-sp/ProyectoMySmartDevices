using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Soportes
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await CargarUsuarios();

            Soporte = new Soporte
            {
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                Respuesta = "Pendiente"
            };

            return Page();
        }

        [BindProperty]
        public Soporte Soporte { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            // Siempre asignar fecha actual
            Soporte.Fecha = DateOnly.FromDateTime(DateTime.Now);

            // Validaciones
            if (string.IsNullOrWhiteSpace(Soporte.Descripcion))
                ModelState.AddModelError("Soporte.Descripcion", "La descripción es obligatoria.");

            if (string.IsNullOrWhiteSpace(Soporte.Tipo))
                ModelState.AddModelError("Soporte.Tipo", "Debe seleccionar un tipo de consulta.");

            if (string.IsNullOrWhiteSpace(Soporte.Respuesta))
                ModelState.AddModelError("Soporte.Respuesta", "Debe seleccionar un método de respuesta.");

            if (Soporte.IdUsuarios <= 0)
                ModelState.AddModelError("Soporte.IdUsuarios", "Debe seleccionar un usuario.");

            if (!ModelState.IsValid)
            {
                await CargarUsuarios();
                return Page();
            }

            try
            {
                _context.Soportes.Add(Soporte);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ticket creado exitosamente.";
                return RedirectToPage("./Index");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Error al crear el ticket.");
                await CargarUsuarios();
                return Page();
            }
        }

        private async Task CargarUsuarios()
        {
            var usuarios = await _context.Usuarios
                .OrderBy(u => u.Nombre)
                .Select(u => new { u.Id, Display = $"{u.Nombre} ({u.Correo})" })
                .ToListAsync();

            ViewData["NombreUsuarios"] = new SelectList(usuarios, "Id", "Display");
        }
    }
}
