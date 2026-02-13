using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata;
using ProyectoMSD.Modelos;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ProyectoMSD.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _db;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext appDbContext)
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
            //HASHEO CONTRASEÑA
             byte[] salt = new byte[0];
            // Crear hash con PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(Password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            Password = Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
            

            var usuario = _db.Usuarios.Where(x => x.Correo == Correo && x.Clave == Password).FirstOrDefault();



            if (usuario != null)
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, Correo),
                        new Claim(ClaimTypes.Role, usuario.Rol) // Usuario o Admin
                    };

                var identity = new ClaimsIdentity(claims,
                                                  CookieAuthenticationDefaults.AuthenticationScheme,
                                                  ClaimTypes.Name,
                                                  ClaimTypes.Role);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);    

                return RedirectToPage("/Usuarios/Index");
            }
            ModelState.AddModelError("", "Usuario o contraseña inválidos");
            return Page();

        }


    }
}
