using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Pages.Soportes
{
    // SEC-01 CORREGIDO: el Edit es accesible para usuarios propios de su ticket y administradores.
    [Authorize(Roles = "Usuario,Admin")]
    public class EditModel : PageModel
    {
        private readonly ISoporteService _soporteService;
        private readonly AppDbContext _context;

        public EditModel(ISoporteService soporteService, AppDbContext context)
        {
            _soporteService = soporteService;
            _context = context;
        }

        /// <summary>
        /// DTO de edición — expone solo los campos permitidos, elimina over-posting.
        /// </summary>
        [BindProperty]
        public EditarSoporteDto Input { get; set; } = new();

        // Datos del soporte para la vista (solo lectura)
        public Soporte Soporte { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var soporte = await _soporteService.ObtenerPorIdAsync(id.Value, incluirUsuario: true);
            if (soporte == null)
                return NotFound();

            Soporte = soporte;

            // Mapear modelo → DTO para pre-poblar el formulario
            Input = new EditarSoporteDto
            {
                Id          = soporte.Id,
                Fecha       = soporte.Fecha,
                Tipo        = soporte.Tipo,
                Descripcion = soporte.Descripcion,
                Respuesta   = soporte.Respuesta,
                IdUsuarios  = soporte.IdUsuarios
            };

            ViewData["IdUsuarios"] = new SelectList(_context.Usuarios, "Id", "Nombre", soporte.IdUsuarios);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validar whitelist del campo Tipo
            if (!_soporteService.EsTipoValido(Input.Tipo))
                ModelState.AddModelError("Input.Tipo", "El tipo de soporte seleccionado no es válido.");

            if (!ModelState.IsValid)
            {
                // Recargar datos del soporte para la vista
                Soporte = (await _soporteService.ObtenerPorIdAsync(Input.Id, incluirUsuario: true))!;
                ViewData["IdUsuarios"] = new SelectList(_context.Usuarios, "Id", "Nombre", Input.IdUsuarios);
                return Page();
            }

            // El servicio sanitiza cada campo y persiste con asignación explícita
            var editado = await _soporteService.EditarAsync(Input);
            if (!editado)
                return NotFound();

            return RedirectToPage("./Index");
        }
    }
}
