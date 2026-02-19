using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Configuración de las páginas Razor
builder.Services.AddRazorPages();

// Configuración de la Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Index";
        options.AccessDeniedPath = "/Denegada";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

        options.ClaimsIssuer = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Cookie.Name = "MySmartDeviceCookie";
    });

// Configurar el DbContext con MySQL usando la versión fija de XAMPP/MariaDB
var connString = builder.Configuration.GetConnectionString("ConexionSQL");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connString, ServerVersion.Parse("10.4-mariadb"))
);

var app = builder.Build();

// ------------------------------------------------------------------
// BLINDAJE DE ARRANQUE: Forzamos la página de errores para depuración 
// y protegemos la conexión inicial a la base de datos.
// ------------------------------------------------------------------
app.UseDeveloperExceptionPage();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Intenta conectar y crear las tablas si no existen
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        // Si Ngrok demora, atrapa el error pero permite que la web cargue
        Console.WriteLine("Error de BD en el arranque: " + ex.Message);
    }
}
// ------------------------------------------------------------------

// Configuración del pipeline HTTP
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();