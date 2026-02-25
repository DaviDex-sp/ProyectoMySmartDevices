using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Servicios de Razor Pages y Autenticación
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Index";
        options.AccessDeniedPath = "/Denegada";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Cookie.Name = "MySmartDeviceCookie";
    });

<<<<<<< HEAD
// Configuración profesional: Leemos la cadena desde el JSON (local) o Variables de Entorno (Azure)
var connString = builder.Configuration.GetConnectionString("ConexionSQL");

// Conectamos a Aiven usando Pomelo configurado para MySQL 8.0
=======
// Configuración del DbContext: Ahora lee la cadena desde Azure y autodetecta la versión
var connString = builder.Configuration.GetConnectionString("ConexionSQL");

>>>>>>> b1580d9aeed806651ac03c50aa3af5cc96101938
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connString, ServerVersion.AutoDetect(connString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null))
);

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();