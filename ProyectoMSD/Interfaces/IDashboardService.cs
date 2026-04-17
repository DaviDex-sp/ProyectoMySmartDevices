using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Interfaces;

public interface IDashboardService
{
    /// <summary>
    /// Obtiene todas las métricas procesadas para el Dashboard Administrativo.
    /// Resuelve el anti-patrón Smart UI evitando que las Razor Pages consulten a la BD directamente.
    /// </summary>
    /// <returns>DashboardMetricsDto con todos los KPIs, gráficos y tablas.</returns>
    Task<DashboardMetricsDto> GetMetricsAsync();

    /// <summary>
    /// Obtiene métricas individuales de sesión por usuario (para la vista de métricas propias del cliente).
    /// </summary>
    Task<DashboardMetricsDto> GetUserMetricsAsync(int userId);

    /// <summary>
    /// Obtiene métricas detalladas de un cliente individual para el panel Big Data.
    /// Cargado vía AJAX desde el modal de métricas del Dashboard Admin.
    /// </summary>
    /// <param name="clienteId">ID del usuario/cliente a evaluar.</param>
    /// <returns>ClienteMetricasDto con KPIs y timeline de movimientos. Null si no existe.</returns>
    Task<ClienteMetricasDto?> GetClienteMetricasAsync(int clienteId);

    /// <summary>
    /// Obtiene el resumen paginado de clientes para la tabla Big Data del Dashboard Admin.
    /// Diseñado para escalar a 10,000+ usuarios sin degradación de rendimiento.
    /// </summary>
    /// <param name="page">Número de página (base 1).</param>
    /// <param name="pageSize">Cantidad de registros por página.</param>
    /// <param name="busqueda">Filtro de texto opcional sobre nombre o correo.</param>
    /// <returns>PagedResultDto con la lista paginada de ClienteResumenDto.</returns>
    Task<PagedResultDto<ClienteResumenDto>> GetResumenClientesAsync(int page, int pageSize, string? busqueda = null);
}

