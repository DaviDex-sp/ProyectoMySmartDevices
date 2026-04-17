using System;
using System.Collections.Generic;

namespace ProyectoMSD.Modelos.DTOs;

/// <summary>
/// Métricas detalladas de un cliente individual para el panel Big Data del Dashboard Admin.
/// Diseñado para ser cargado vía AJAX desde el handler OnGetClienteMetricasAsync.
/// </summary>
public class ClienteMetricasDto
{
    // --- Identidad ---
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Inicial { get; set; } = string.Empty;

    // --- KPIs Individuales ---
    public int TotalPropiedades { get; set; }
    public int TotalEspacios { get; set; }
    public int TotalDispositivos { get; set; }
    public int TotalSoportes { get; set; }
    public int SoportesPendientes { get; set; }
    public int TotalLogins { get; set; }
    public DateTime? UltimoAcceso { get; set; }

    // --- Historial de Movimientos (Timeline de eventos de negocio) ---
    public List<MovimientoDto> Movimientos { get; set; } = new();
}

/// <summary>
/// Representa un evento de negocio rastreable derivado del RegistroAcceso.
/// Tipo de acciones: Login, Logout, PageView con path semántico (crear propiedad, etc.)
/// </summary>
public class MovimientoDto
{
    /// <summary>Fecha y hora exacta del evento.</summary>
    public DateTime Fecha { get; set; }

    /// <summary>Tipo semántico del evento: Login, Logout, Navegación, etc.</summary>
    public string TipoAccion { get; set; } = string.Empty;

    /// <summary>Descripción legible del movimiento para mostrar en la timeline.</summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Clase de ícono Font Awesome (ej: fa-sign-in-alt).</summary>
    public string Icono { get; set; } = string.Empty;

    /// <summary>Clase CSS de color temático (ej: text-success, text-primary).</summary>
    public string ColorClase { get; set; } = string.Empty;
}
