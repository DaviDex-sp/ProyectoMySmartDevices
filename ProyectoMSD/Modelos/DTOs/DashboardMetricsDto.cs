using System;
using System.Collections.Generic;

namespace ProyectoMSD.Modelos.DTOs;

/// <summary>
/// DTO principal de métricas del Dashboard Administrativo.
/// Centraliza todos los KPIs, datos de gráficos y tablas de resumen.
/// Nunca expone entidades del dominio directamente (Separación de Concerns).
/// </summary>
public class DashboardMetricsDto
{
    // --- KPIs Generales ---
    public int TotalUsuarios { get; set; }
    public int TotalDispositivos { get; set; }
    public int TotalPropiedades { get; set; }
    public int TotalEspacios { get; set; }
    public int TotalSoportes { get; set; }
    public int SoportesPendientes { get; set; }
    public int TotalConfiguraciones { get; set; }
    public int IngresosUltimoMes { get; set; }
    public double TiempoPromedioSesionMinutos { get; set; }

    // --- KPIs Admin Ampliados (Big Data) ---
    public int UsuariosActivos { get; set; }
    public int UsuariosInactivos { get; set; }
    public int UsuariosConPropiedades { get; set; }
    public int UsuariosSinPropiedades { get; set; }

    // --- Datos para Chart.js ---
    public List<ChartDataDto> NavegadoresUsados { get; set; } = new();
    public List<ChartDataDto> PaginasMasVisitadas { get; set; } = new();
    public List<ChartDataDto> UsuariosPorUbicacion { get; set; } = new();
    public List<ChartDataDto> UsuariosPorRol { get; set; } = new();
    public List<ChartDataDto> AccesosPorDia { get; set; } = new();
    public List<ChartDataDto> DispositivosPorTipo { get; set; } = new();

    // --- Tablas de Acceso Rápido ---
    public List<RegistroAccesoResumenDto> UltimosAccesos { get; set; } = new();

    /// <summary>
    /// MIGRADO: Reemplaza el anti-patrón List&lt;Soporte&gt; (entidad de dominio cruda).
    /// Ahora usa SoporteResumenDto para mantener separación de capas.
    /// </summary>
    public List<SoporteResumenDto> UltimosSoportes { get; set; } = new();
}

/// <summary>
/// DTO de resumen de un soporte para tablas del Dashboard.
/// Reemplaza la exposición directa de la entidad Soporte.
/// </summary>
public class SoporteResumenDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public bool EstaRespondido { get; set; }
}

/// <summary>
/// DTO de resumen de un cliente para la tabla Big Data del Dashboard Admin.
/// Incluye KPIs consolidados de todas sus entidades relacionadas.
/// </summary>
public class ClienteResumenDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Inicial { get; set; } = string.Empty;
    public int Propiedades { get; set; }
    public int Espacios { get; set; }
    public int Dispositivos { get; set; }
    public int Soportes { get; set; }
    public int Logins { get; set; }
    public DateTime? UltimoAcceso { get; set; }
}

/// <summary>DTO para datasets de Chart.js (label + count).</summary>
public class ChartDataDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>DTO de resumen de un registro de acceso para tablas del Dashboard.</summary>
public class RegistroAccesoResumenDto
{
    public string NombreUsuario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string TipoAccion { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
}
