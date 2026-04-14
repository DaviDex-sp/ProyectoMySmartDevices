using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoMSD.Modelos;
using ProyectoMSD.Interfaces;
using System;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages
{
    public class RegistrarModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly AppDbContext _context;

        public RegistrarModel(IUsuarioService usuarioService, AppDbContext context)
        {
            _usuarioService = usuarioService;
            _context = context;
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;

        [BindProperty]
        public decimal Latitud { get; set; }

        [BindProperty]
        public decimal Longitud { get; set; }

        [BindProperty]
        public string? DireccionFormateada { get; set; }

        public void OnGet()
        {
            Usuario = new Usuario
            {
                Permisos = "limitado",    
                Acesso = "pendiente"      
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await ValidarDatos();

            if (!ModelState.IsValid)
            {
                return Page();
            }
          
            try
            {
                LimpiarDatos();

                // 1. CREAR Y GUARDAR LA UBICACIÓN PRIMERO EN MYSQL
                // Aplicamos Math.Round para que los decimales coincidan con decimal(10,8) y decimal(11,8) de tu BD
                var nuevaUbicacion = new Ubicacione
                {
                    Latitud = Math.Round(this.Latitud, 8),
                    Longitud = Math.Round(this.Longitud, 8),
                    DireccionFormateada = this.DireccionFormateada,
                    FechaCreacion = DateTime.UtcNow
                };
                
                _context.Ubicaciones.Add(nuevaUbicacion);
                await _context.SaveChangesAsync();

                // 2. FORZAR VALORES DE SEGURIDAD DEL USUARIO
                Usuario.Permisos = "limitado";      
                Usuario.Acesso = "pendiente";       
                
                // 3. ENLAZAR LA LLAVE FORÁNEA
                Usuario.IdUbicacion = nuevaUbicacion.Id; 
                
                // 4. GUARDAR EL USUARIO COMPLETO
                await _usuarioService.CreateUsuarioAsync(Usuario);

                TempData["SuccessMessage"] = "¡Cuenta creada exitosamente! Tu cuenta está pendiente de activación por un administrador.";
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- ERROR CRÍTICO EN DB ---");
                Console.WriteLine(ex.Message);
                if(ex.InnerException != null) Console.WriteLine($"DETALLE: {ex.InnerException.Message}");
                Console.WriteLine($"---------------------------\n");

                ModelState.AddModelError(string.Empty, "Error en la base de datos al crear la cuenta. Revisa la consola de Visual Studio para más detalles.");
                return Page();
            }
        }

        private async Task ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(Usuario.Nombre))
                ModelState.AddModelError("Usuario.Nombre", "El nombre es obligatorio.");
            else if (Usuario.Nombre.Length < 2)
                ModelState.AddModelError("Usuario.Nombre", "El nombre debe tener al menos 2 caracteres.");
            else if (Usuario.Nombre.Length > 100)
                ModelState.AddModelError("Usuario.Nombre", "El nombre no puede exceder 100 caracteres.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(Usuario.Nombre, @"^[a-zA-ZñÑáéíóúÁÉÍÓÚ\s]+$"))
                ModelState.AddModelError("Usuario.Nombre", "El nombre solo puede contener letras y espacios.");

            if (string.IsNullOrWhiteSpace(Usuario.Correo))
                ModelState.AddModelError("Usuario.Correo", "El correo es obligatorio.");
            else if (!EsEmailValido(Usuario.Correo))
                ModelState.AddModelError("Usuario.Correo", "El formato del correo no es válido.");
            else if (Usuario.Correo.Length > 200)
                ModelState.AddModelError("Usuario.Correo", "El correo no puede exceder 200 caracteres.");
            else
            {
                var existeCorreo = await _usuarioService.ExisteCorreoAsync(Usuario.Correo);
                if (existeCorreo)
                    ModelState.AddModelError("Usuario.Correo", "Ya existe un usuario con este correo.");
            }

            if (string.IsNullOrWhiteSpace(Usuario.Clave))
                ModelState.AddModelError("Usuario.Clave", "La contraseña es obligatoria.");
            else if (Usuario.Clave.Length < 6)
                ModelState.AddModelError("Usuario.Clave", "La contraseña debe tener al menos 6 caracteres.");
            else if (Usuario.Clave.Length > 50)
                ModelState.AddModelError("Usuario.Clave", "La contraseña no puede exceder 50 caracteres.");

            if (string.IsNullOrWhiteSpace(Usuario.Telefono))
                ModelState.AddModelError("Usuario.Telefono", "El teléfono debe ser un número válido.");
            else if (Usuario.Telefono.Trim().Length < 7)
                ModelState.AddModelError("Usuario.Telefono", "El teléfono debe tener al menos 7 dígitos.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(Usuario.Telefono.Trim(), @"^\d+$"))
                ModelState.AddModelError("Usuario.Telefono", "El teléfono solo puede contener números.");

            if (string.IsNullOrWhiteSpace(DireccionFormateada))
                ModelState.AddModelError("DireccionFormateada", "Debes seleccionar tu ubicación en el mapa y completar la dirección.");
            else if (DireccionFormateada.Length < 5)
                ModelState.AddModelError("DireccionFormateada", "La dirección proporcionada es muy corta.");

            if (!string.IsNullOrWhiteSpace(Usuario.Documento))
            {
                if (Usuario.Documento.Length < 6)
                    ModelState.AddModelError("Usuario.Documento", "El documento debe tener al menos 6 dígitos.");
                else if (!System.Text.RegularExpressions.Regex.IsMatch(Usuario.Documento, @"^\d+$"))
                    ModelState.AddModelError("Usuario.Documento", "El documento solo puede contener números.");
                else
                {
                    var existeDocumento = await _usuarioService.ExisteDocumentoAsync(Usuario.Documento);
                    if (existeDocumento)
                        ModelState.AddModelError("Usuario.Documento", "Ya existe un usuario con este documento.");
                }
            }

            if (!string.IsNullOrWhiteSpace(Usuario.Rut))
            {
                if (Usuario.Rut.Length < 8)
                    ModelState.AddModelError("Usuario.Rut", "El RUT debe tener al menos 8 caracteres.");
                else if (Usuario.Rut.Length > 50)
                    ModelState.AddModelError("Usuario.Rut", "El RUT no puede exceder 50 caracteres.");
                else if (!System.Text.RegularExpressions.Regex.IsMatch(Usuario.Rut, @"^[a-zA-Z0-9\-]+$"))
                    ModelState.AddModelError("Usuario.Rut", "El RUT solo puede contener caracteres alfanuméricos y guiones.");
            }

            if (string.IsNullOrWhiteSpace(Usuario.Rol))
                ModelState.AddModelError("Usuario.Rol", "Debe seleccionar un rol.");
        }

        private void LimpiarDatos()
        {
            Usuario.Nombre = SanitizarTextoBasico(Usuario.Nombre);
            Usuario.Correo = Usuario.Correo?.Trim().ToLower();
            Usuario.Rut = string.IsNullOrWhiteSpace(Usuario.Rut) ? null : SanitizarAlfanumerico(Usuario.Rut);
            Usuario.Documento = string.IsNullOrWhiteSpace(Usuario.Documento) ? null : SanitizarSoloNumeros(Usuario.Documento);
            Usuario.Telefono = string.IsNullOrWhiteSpace(Usuario.Telefono) ? null : SanitizarSoloNumeros(Usuario.Telefono);
            Usuario.Clave = Usuario.Clave?.Trim(); 
            DireccionFormateada = SanitizarTextoBasico(DireccionFormateada);
        }

        private string? SanitizarTextoBasico(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            var limpio = input.Trim();
            // Remover posibles etiquetas HTML/Script para evitar XSS persistente
            return System.Text.RegularExpressions.Regex.Replace(limpio, "<.*?>", string.Empty);
        }

        private string? SanitizarAlfanumerico(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            var limpio = input.Trim();
            return System.Text.RegularExpressions.Regex.Replace(limpio, "[^a-zA-Z0-9\\-]", string.Empty);
        }

        private string? SanitizarSoloNumeros(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            var limpio = input.Trim();
            return System.Text.RegularExpressions.Regex.Replace(limpio, "[^0-9]", string.Empty);
        }

        private bool EsEmailValido(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}