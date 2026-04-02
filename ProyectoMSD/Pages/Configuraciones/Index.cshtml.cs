using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;
using System.Security.Claims;

namespace ProyectoMSD.Pages.Configuraciones;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IConfiguracionService _configService;

    public IndexModel(IConfiguracionService configService)
    {
        _configService = configService;
    }

    public IList<Configuracione> Configuracione { get; set; } = default!;
    public int TotalNotificaciones { get; set; }
    public int DispositivosActivos { get; set; }
    public string UltimaSincronizacion { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        // Obtener el ID del usuario actual de forma segura
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        bool esAdmin = User.IsInRole("Admin") || User.IsInRole("Administrador");

        if (esAdmin)
        {
            Configuracione = await _configService.ObtenerTodasAsync();
        }
        else if (int.TryParse(userIdClaim, out int userId))
        {
            Configuracione = await _configService.ObtenerPorUsuarioAsync(userId);
        }
        else
        {
            Configuracione = new List<Configuracione>();
        }

        // Obtener métricas desde el servicio
        TotalNotificaciones = await _configService.ObtenerConteoNotificacionesAsync();
        DispositivosActivos = await _configService.ObtenerConteoDispositivosAsync();
        UltimaSincronizacion = DateTime.Now.AddMinutes(-5).ToString("dd/MM/yyyy HH:mm");
    }
}
