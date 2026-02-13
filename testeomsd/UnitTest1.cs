using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProyectoMSD.Modelos;
using ProyectoMSD.Pages;
using System.Security.Claims;
using System.Security.Cryptography;
using Moq;
using ProyectoMSD.Pages.Dispositivos;

namespace ProyectoMSD.Models
{
    public class LoginTest
    {
        // Ejemplo de PageModel simplificado
        public class LoginPageModel : PageModel
        {
            public string Correo { get; set; } = "";
            public Usuario? UsuarioEncontrado { get; set; }  // en vez de ir a la DB para el test

            public async Task<IActionResult> OnPostAsync()
            {
                var usuario = UsuarioEncontrado;

                if (usuario != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

                    var identity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name,
                        ClaimTypes.Role);

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal);

                    return RedirectToPage("/Usuarios/Index");
                }

                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos");
                return Page();
            }
        }

        public class Usuario
        {
            public string Correo { get; set; } = "";
            public string Rol { get; set; } = "";
        }

        public class LoginTests
        {
            // Configura un HttpContext con IAuthenticationService "mockeado"
            private static DefaultHttpContext CrearHttpContextConAuthMock()
            {
                var services = new ServiceCollection();

                var authMock = new Mock<IAuthenticationService>();
                authMock
                    .Setup(a => a.SignInAsync(
                        It.IsAny<HttpContext>(),
                        It.IsAny<string>(),
                        It.IsAny<ClaimsPrincipal>(),
                        It.IsAny<AuthenticationProperties>()))
                    .Returns(Task.CompletedTask);

                services.AddSingleton<IAuthenticationService>(authMock.Object);

                var serviceProvider = services.BuildServiceProvider();

                return new DefaultHttpContext
                {
                    RequestServices = serviceProvider
                };
            }

            [Fact]
            public async Task OnPostAsync_UsuarioValido_RedireccionaAUsuariosIndex()
            {
                // Arrange
                var httpContext = CrearHttpContextConAuthMock();

                var page = new LoginPageModel
                {
                    Correo = "test@correo.com",
                    UsuarioEncontrado = new Usuario
                    {
                        Correo = "test@correo.com",
                        Rol = "Admin"
                    }
                };

                page.PageContext = new PageContext
                {
                    HttpContext = httpContext
                };

                // Act
                var result = await page.OnPostAsync();

                // Assert
                var redirect = Assert.IsType<RedirectToPageResult>(result);
                Assert.Equal("/Usuarios/Index", redirect.PageName);
            }
        }
    }
    public class CrearDispositivoTests
    {
        private AppDbContext CrearContextoEnMemoria()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task OnPostAsync_ModelValido_GuardaYRedirige()
        {
            // Arrange
            using var context = CrearContextoEnMemoria();

            var page = new ProyectoMSD.Pages.Dispositivos.CreateModel(context)
            {
                Dispositivo = new Dispositivo
                {
                    // Completa aquí las propiedades requeridas de tu entidad
                    Nombre = "TV Lenovo",
                    Tipo = "television",
                    Usos = "Personal",
                    Marca = "Lenovo",
                    Estado = "activo"
                }
            };

            page.PageContext = new PageContext(); // para que ModelState exista

            // Act
            var result = await page.OnPostAsync();

            // Assert: redirige al Index
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirect.PageName);

            // Assert: el dispositivo se guardó en la BD en memoria
            Assert.Equal(1, context.Dispositivos.Count());
            var disp = context.Dispositivos.First();
            Assert.Equal("TV Lenovo", disp.Nombre);
        }

    }
}
