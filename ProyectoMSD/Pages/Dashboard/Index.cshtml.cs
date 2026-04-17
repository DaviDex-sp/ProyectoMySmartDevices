using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos.DTOs;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Dashboard
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IDashboardService _dashboardService;

        public IndexModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // --- Métricas Globales ---
        public DashboardMetricsDto Metrics { get; set; } = new();

        // --- JSON para Chart.js ---
        public string NavegadoresJson      { get; set; } = "{}";
        public string PaginasJson          { get; set; } = "{}";
        public string UbicacionJson        { get; set; } = "{}";
        public string RolJson              { get; set; } = "{}";
        public string AccesosDiaJson       { get; set; } = "{}";
        public string DispositivosTipoJson { get; set; } = "{}";

        // --- Big Data: Tabla paginada de clientes ---
        public PagedResultDto<ClienteResumenDto> PaginaClientes { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int PageNum { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string? Busqueda { get; set; }

        private const int PAGE_SIZE = 10;

        // =====================================================================
        //  HANDLER GET: Carga principal de la página
        // =====================================================================
        public async Task OnGetAsync()
        {
            Metrics = await _dashboardService.GetMetricsAsync();

            // Serialización para Chart.js
            NavegadoresJson = JsonSerializer.Serialize(new {
                labels = Metrics.NavegadoresUsados.Select(x => x.Label),
                data   = Metrics.NavegadoresUsados.Select(x => x.Count)
            });

            PaginasJson = JsonSerializer.Serialize(new {
                labels = Metrics.PaginasMasVisitadas.Select(x => x.Label),
                data   = Metrics.PaginasMasVisitadas.Select(x => x.Count)
            });

            UbicacionJson = JsonSerializer.Serialize(new {
                labels = Metrics.UsuariosPorUbicacion.Select(x => x.Label),
                data   = Metrics.UsuariosPorUbicacion.Select(x => x.Count)
            });

            RolJson = JsonSerializer.Serialize(new {
                labels = Metrics.UsuariosPorRol.Select(x => x.Label),
                data   = Metrics.UsuariosPorRol.Select(x => x.Count)
            });

            AccesosDiaJson = JsonSerializer.Serialize(new {
                labels = Metrics.AccesosPorDia.Select(x => x.Label),
                data   = Metrics.AccesosPorDia.Select(x => x.Count)
            });

            DispositivosTipoJson = JsonSerializer.Serialize(new {
                labels = Metrics.DispositivosPorTipo.Select(x => x.Label),
                data   = Metrics.DispositivosPorTipo.Select(x => x.Count)
            });

            // Tabla Big Data paginada
            PaginaClientes = await _dashboardService.GetResumenClientesAsync(PageNum, PAGE_SIZE, Busqueda);
        }

        // =====================================================================
        //  HANDLER AJAX: Métricas individuales de un cliente (para modal)
        // =====================================================================
        public async Task<IActionResult> OnGetClienteMetricasAsync(int clienteId)
        {
            var metricas = await _dashboardService.GetClienteMetricasAsync(clienteId);
            if (metricas == null) return NotFound();

            return new JsonResult(metricas, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        // =====================================================================
        //  HANDLER AJAX: Tabla paginada de clientes (para navegación sin reload)
        // =====================================================================
        public async Task<IActionResult> OnGetResumenClientesAsync(int page = 1, string? busqueda = null)
        {
            var resultado = await _dashboardService.GetResumenClientesAsync(page, PAGE_SIZE, busqueda);

            return new JsonResult(resultado, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
