using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Pages.Soportes
{
    [Authorize(Roles = "Usuario")]
    public class CreateModel : PageModel
    {
        private readonly ISoporteService _soporteService;
        private readonly AppDbContext _context;

        public CreateModel(ISoporteService soporteService, AppDbContext context)
        {
            _soporteService = soporteService;
            _context = context;
        }

        /// <summary>
        /// DTO de entrada — previene over-posting al exponer solo Tipo y Descripcion.
        /// </summary>
        [BindProperty]
        public CrearSoporteDto Input { get; set; } = new();

        // Datos del usuario en sesión (solo para mostrar en la vista)
        public Usuario UsuarioSesion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var usuarioId = ObtenerIdUsuarioSesion();
            if (usuarioId == null)
                return RedirectToPage("/Index");

            var usuario = await _context.Usuarios.FindAsync(usuarioId.Value);
            if (usuario == null)
                return RedirectToPage("/Index");

            UsuarioSesion = usuario;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var usuarioId = ObtenerIdUsuarioSesion();
            if (usuarioId == null)
                return RedirectToPage("/Index");

            // Validar whitelist del campo Tipo antes de continuar
            if (!_soporteService.EsTipoValido(Input.Tipo))
                ModelState.AddModelError("Input.Tipo", "El tipo de consulta seleccionado no es válido.");

            if (!ModelState.IsValid)
            {
                UsuarioSesion = (await _context.Usuarios.FindAsync(usuarioId.Value))!;
                return Page();
            }

            try
            {
                // Cargar datos del usuario para la notificación
                UsuarioSesion = (await _context.Usuarios.FindAsync(usuarioId.Value))!;

                // El servicio aplica sanitización y persiste el ticket
                var creado = await _soporteService.CrearAsync(Input, usuarioId.Value);
                if (!creado)
                {
                    ModelState.AddModelError(string.Empty, "No se pudo crear el ticket. Tipo de consulta inválido.");
                    return Page();
                }

                // Obtener el ticket recién creado para la notificación
                var nuevoTicket = await _context.Soportes
                    .Where(s => s.IdUsuarios == usuarioId.Value)
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefaultAsync();

                // Notificar a todos los administradores
                var administradores = await _context.Usuarios
                    .Where(u => u.Rol == "Admin")
                    .ToListAsync();

                foreach (var admin in administradores)
                {
                    _context.Notificaciones.Add(new Notificacion
                    {
                        IdUsuarios      = admin.Id,
                        Titulo          = "Nuevo Ticket de Soporte",
                        Mensaje         = $"El usuario {UsuarioSesion.Nombre} ha creado un nuevo ticket de tipo {Input.Tipo}.",
                        Tipo            = "NuevoTicket",
                        Leida           = false,
                        FechaCreacion   = DateTime.Now,
                        RutaRedireccion = nuevoTicket != null
                            ? $"/Soportes/Responder/{nuevoTicket.Id}"
                            : "/Soportes/Index"
                    });
                }

                if (administradores.Any())
                    await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ticket creado exitosamente.";
                return RedirectToPage("./Index");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Error al crear el ticket. Intenta nuevamente.");
                UsuarioSesion = (await _context.Usuarios.FindAsync(usuarioId.Value))!;
                return Page();
            }
        }

        private int? ObtenerIdUsuarioSesion()
        {
            var claim = User.FindFirst("UserId");
            if (claim != null && int.TryParse(claim.Value, out int id))
                return id;
            return null;
        }
    }
}
