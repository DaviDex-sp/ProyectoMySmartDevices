using System;
using System.Collections.Generic;

namespace ProyectoMSD.Modelos.DTOs;

/// <summary>
/// DTO genérico de paginación para cualquier colección de datos en el sistema.
/// Diseñado para escalar a 10,000+ registros sin degradación de rendimiento.
/// Uso: GetResumenClientesAsync(page, pageSize) → PagedResultDto&lt;ClienteResumenDto&gt;
/// </summary>
/// <typeparam name="T">Tipo del DTO a paginar.</typeparam>
public class PagedResultDto<T>
{
    /// <summary>Elementos de la página actual.</summary>
    public List<T> Items { get; set; } = new();

    /// <summary>Total de registros en la base de datos (sin paginar).</summary>
    public int TotalItems { get; set; }

    /// <summary>Página actual (base 1).</summary>
    public int Page { get; set; }

    /// <summary>Cantidad de registros por página.</summary>
    public int PageSize { get; set; }

    /// <summary>Total de páginas calculado.</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 0;

    /// <summary>Indica si existe una página anterior.</summary>
    public bool HasPrevious => Page > 1;

    /// <summary>Indica si existe una página siguiente.</summary>
    public bool HasNext => Page < TotalPages;

    /// <summary>Índice del primer elemento de la página actual (base 1).</summary>
    public int From => TotalItems == 0 ? 0 : (Page - 1) * PageSize + 1;

    /// <summary>Índice del último elemento de la página actual.</summary>
    public int To => Math.Min(Page * PageSize, TotalItems);
}
