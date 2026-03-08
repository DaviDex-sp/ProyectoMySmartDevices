using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata;
using ProyectoMSD.Modelos;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Google;

namespace ProyectoMSD.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(ILogger<LoginModel> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _db = appDbContext;
        }

        [BindProperty]
        public string Correo { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Hasheo de contraseña con PBKDF2
            byte[] salt = new byte[0];
            var pbkdf2 = new Rfc2898DeriveBytes(Password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            Password = Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);

            var usuario = _db.Usuarios
                .Where(x => x.Correo == Correo && x.Clave == Password)
                .FirstOrDefault();

            if (usuario != null)
            {
                // Crear claims de autenticacion
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Correo),
                    new Claim(ClaimTypes.Role, usuario.Rol),
                    new Claim("UserId", usuario.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name,
                    ClaimTypes.Role);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Registrar el acceso en la tabla de seguimiento
                try
                {
                    var registro = new RegistroAcceso
                    {
                        IdUsuarios = usuario.Id,
                        FechaAcceso = DateTime.Now,
                        TipoAccion = "Login",
                        DireccionIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Navegador = Request.Headers["User-Agent"].ToString(),
                        PaginaVisitada = "/Login"
                    };
                    _db.RegistroAccesos.Add(registro);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // No bloquear el login si falla el registro
                    _logger.LogWarning(ex, "No se pudo registrar el acceso del usuario {UserId}", usuario.Id);
                }

                return RedirectToPage("/Usuarios/Index");
            }

            ModelState.AddModelError("", "Usuario o contrasena invalidos");
            return Page();
        }

        public IActionResult OnPostGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Page("./Login", pageHandler: "GoogleCallback") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> OnGetGoogleCallbackAsync()
        {
            // Leer la información del usuario desde la cookie temporal
            var result = await HttpContext.AuthenticateAsync("ExternalCookie");
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error al autenticarse con Google.");
                return Page();
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "No se pudo recuperar el correo electrónico de Google.");
                return Page();
            }

            // Buscar usuario en DB
            var usuario = _db.Usuarios.FirstOrDefault(x => x.Correo == email);

            if (usuario == null)
            {
                // Crear nuevo usuario con valores por defecto
                usuario = new Usuario
                {
                    Nombre = name ?? "Usuario Google",
                    Correo = email,
                    Clave = "[Autenticado por Google]",
                    Rol = "Usuario",
                    Telefono = 0,
                    Ubicacion = "Por Defecto",
                    Permisos = "Basico",
                    Acesso = "Activo",
                    Documento = 0
                };

                _db.Usuarios.Add(usuario);
                await _db.SaveChangesAsync();
            }

            // Crear claims y hacer SignIn local
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("UserId", usuario.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Borrar la cookie temporal de Google
            await HttpContext.SignOutAsync("ExternalCookie");

            // Registrar el acceso en la tabla de seguimiento
            try
            {
                var registro = new RegistroAcceso
                {
                    IdUsuarios = usuario.Id,
                    FechaAcceso = DateTime.Now,
                    TipoAccion = "Login Google",
                    DireccionIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Navegador = Request.Headers["User-Agent"].ToString(),
                    PaginaVisitada = "/Login"
                };
                _db.RegistroAccesos.Add(registro);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo registrar el acceso de Google del usuario {UserId}", usuario.Id);
            }

            return RedirectToPage("/Usuarios/Index");
        }
    }
}
