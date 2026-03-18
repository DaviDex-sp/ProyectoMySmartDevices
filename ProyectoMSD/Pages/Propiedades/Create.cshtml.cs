using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Propiedades
{
    public class CreateModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public CreateModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // CORREGIDO: Mostrar nombre del usuario, no el ID
            await LoadUsuariosDropdown();
            return Page();
        }

        [BindProperty]
        public Propiedade Propiedade { get; set; } = default!;

        [BindProperty]
        public int IdUsuarioSeleccionado { get; set; }

        [BindProperty]
        public decimal? Latitud { get; set; }

        [BindProperty]
        public decimal? Longitud { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // DEBUGGING - Verificar qué datos llegan
            Console.WriteLine($"===== DEBUGGING DATOS =====");
            Console.WriteLine($"IdUsuarioSeleccionado: {IdUsuarioSeleccionado}");
            Console.WriteLine($"Direccion: {Propiedade.Direccion}");
            Console.WriteLine($"Tipo: {Propiedade.Tipo}");
            Console.WriteLine($"Pisos: {Propiedade.Pisos}");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            // VALIDACIÓN ADICIONAL para la llave foránea
            if (IdUsuarioSeleccionado <= 0)
            {
                ModelState.AddModelError("IdUsuarioSeleccionado", "Debe seleccionar un usuario válido");
            }
            else
            {
                // VERIFICAR que el usuario existe en la base de datos
                var usuarioExists = await _context.Usuarios
                    .AnyAsync(u => u.Id == IdUsuarioSeleccionado);

                if (!usuarioExists)
                {
                    ModelState.AddModelError("IdUsuarioSeleccionado", "El usuario seleccionado no existe");
                }
            }

            if (!ModelState.IsValid)
            {
                // CRÍTICO: Re-cargar el dropdown cuando hay errores
                await LoadUsuariosDropdown();

                // DEBUGGING - Mostrar errores
                foreach (var modelError in ModelState)
                {
                    foreach (var error in modelError.Value.Errors)
                    {
                        Console.WriteLine($"Error en {modelError.Key}: {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            try
            {
                Console.WriteLine("===== INTENTANDO GUARDAR =====");
                
                // Si el usuario usó el mapa, creamos el registro de Ubicacione
                if (Latitud.HasValue && Longitud.HasValue && !string.IsNullOrEmpty(Propiedade.Direccion))
                {
                    Propiedade.IdUbicacionNavigation = new Ubicacione
                    {
                        Latitud = Latitud.Value,
                        Longitud = Longitud.Value,
                        DireccionFormateada = Propiedade.Direccion,
                        FechaCreacion = DateTime.Now
                    };
                }

                Propiedade.UsuariosPropiedades.Add(new UsuariosPropiedade
                { 
                    IdUsuario = IdUsuarioSeleccionado, 
                    RolEnPropiedad = "Propietario" 
                });

                _context.Propiedades.Add(Propiedade);

                // NOTIFICACIÓN PARA EL USUARIO ASIGNADO
                _context.Notificaciones.Add(new Notificacion
                {
                    IdUsuarios = IdUsuarioSeleccionado,
                    Titulo = "Nueva Propiedad Asignada",
                    Mensaje = $"Se te ha asignado una nueva propiedad: {Propiedade.Direccion}",
                    Tipo = "Informacion",
                    FechaCreacion = DateTime.Now,
                    Leida = false,
                    RutaRedireccion = "/Propiedades/Index"
                });

                // NOTIFICACIÓN PARA EL ADMINISTRADOR (CREADOR)
                var adminIdClaim = User.FindFirst("UserId")?.Value;
                if (int.TryParse(adminIdClaim, out int adminId))
                {
                    _context.Notificaciones.Add(new Notificacion
                    {
                        IdUsuarios = adminId,
                        Titulo = "Propiedad Creada Exitosamente",
                        Mensaje = $"Has registrado la propiedad: {Propiedade.Direccion}",
                        Tipo = "Exito",
                        FechaCreacion = DateTime.Now,
                        Leida = false,
                        RutaRedireccion = "/Propiedades/Index"
                    });
                }

                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"Filas afectadas: {result}");
                Console.WriteLine("===== GUARDADO EXITOSO =====");

                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"===== ERROR AL GUARDAR =====");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                // MOSTRAR error específico de llave foránea
                if (ex.InnerException?.Message?.Contains("FOREIGN KEY") == true)
                {
                    ModelState.AddModelError("IdUsuarioSeleccionado", "Error: El usuario seleccionado no es válido");
                }
                else
                {
                    ModelState.AddModelError("", $"Error al guardar: {ex.InnerException?.Message ?? ex.Message}");
                }

                await LoadUsuariosDropdown();
                return Page();
            }
        }

        private async Task LoadUsuariosDropdown()
        {
            // MÉTODO CORRECTO: Mostrar nombre y correo, valor = Id
            var usuarios = await _context.Usuarios
                .Where(u => u.Id > 0) // Solo usuarios válidos
                .OrderBy(u => u.Nombre)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Nombre} - {u.Correo}" // ESTO es lo que ve el usuario
                })
                .ToListAsync();

            ViewData["IdUsuarios"] = new SelectList(usuarios, "Value", "Text");

            // DEBUGGING - Verificar que se cargaron usuarios
            Console.WriteLine($"Usuarios cargados en dropdown: {usuarios.Count}");
        }
    }
}

