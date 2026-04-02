
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using ProyectoMSD.Interfaces;

namespace ProyectoMSD.Pages.Usuarios
{
    [Authorize(Roles = "Admin,Usuario,Huesped")]
    public class IndexModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IDashboardService _dashboardService;

        public IndexModel(IUsuarioService usuarioService, IDashboardService dashboardService)
        {
            _usuarioService = usuarioService;
            _dashboardService = dashboardService;
        }

        // Datos del Dashboard
        public IList<Usuario> Usuario { get; set; } = new List<Usuario>();
        public List<RegistroAcceso> UltimosAccesos { get; set; } = new List<RegistroAcceso>();
        public ProyectoMSD.Modelos.DTOs.DashboardMetricsDto Metricas { get; set; } = null!;

        public bool RequiereInformacionBasica { get; set; }
        public bool EsAdmin => User.IsInRole("Admin") || User.IsInRole("Administrador");

        public async Task OnGetAsync()
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdStr, out int userId);

            if (EsAdmin)
            {
                // El Admin ve todo globalmente
                Usuario = await _usuarioService.GetAllUsuariosAsync();
                Metricas = await _dashboardService.GetMetricsAsync();
                UltimosAccesos = await _usuarioService.GetRecentAccessLogsAsync(null, 5);
            }
            else if (userId > 0)
            {
                // Los usuarios estándar solo ven sus propias métricas y accesos
                Metricas = await _dashboardService.GetUserMetricsAsync(userId);
                UltimosAccesos = await _usuarioService.GetRecentAccessLogsAsync(userId, 5);
                
                var profile = await _usuarioService.GetUsuarioPerfilAsync(userId);
                if (profile != null)
                {
                    // Validación de datos básicos
                    if (string.IsNullOrWhiteSpace(profile.Documento) || profile.Documento == "0" || 
                        string.IsNullOrWhiteSpace(profile.Telefono) || profile.Telefono == "0")
                    {
                        RequiereInformacionBasica = true;
                    }
                }
            }
        }
    }
}

