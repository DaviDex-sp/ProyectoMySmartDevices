using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;
using ProyectoMSD.Modelos.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoMSD.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    // =====================================================================
    //  MÉTODO 1: Métricas Globales del Dashboard Admin
    // =====================================================================
    public async Task<DashboardMetricsDto> GetMetricsAsync()
    {
        var dto = new DashboardMetricsDto();

        // 1. KPIs Generales Básicos
        dto.TotalUsuarios        = await _db.Usuarios.CountAsync();
        dto.TotalDispositivos    = await _db.Dispositivos.CountAsync();
        dto.TotalPropiedades     = await _db.Propiedades.CountAsync();
        dto.TotalEspacios        = await _db.Espacios.CountAsync();
        dto.TotalSoportes        = await _db.Soportes.CountAsync();
        dto.TotalConfiguraciones = await _db.Configuraciones.CountAsync();

        dto.SoportesPendientes = await _db.Soportes
            .Where(s => string.IsNullOrEmpty(s.Respuesta))
            .CountAsync();

        // 2. Ingresos del último mes
        var haceUnMes = DateTime.Now.AddMonths(-1);
        dto.IngresosUltimoMes = await _db.RegistroAccesos
            .Where(r => r.FechaAcceso >= haceUnMes && r.TipoAccion == "Login")
            .CountAsync();

        // 3. Tiempo Promedio de Sesión
        var tiempos = await _db.RegistroAccesos
            .Where(r => r.DuracionSesion != null && r.DuracionSesion > 0 && r.DuracionSesion < 86400)
            .Select(r => r.DuracionSesion!.Value)
            .ToListAsync();

        if (tiempos.Any())
        {
            double promedioSegundos = tiempos.Average();
            dto.TiempoPromedioSesionMinutos = Math.Round(promedioSegundos / 60.0, 1);
        }

        // 4. KPIs Admin Ampliados — Big Data
        dto.UsuariosActivos = await _db.Usuarios
            .Where(u => u.Acesso != null && u.Acesso.ToLower() == "activo")
            .CountAsync();

        dto.UsuariosInactivos = await _db.Usuarios
            .Where(u => u.Acesso != null && u.Acesso.ToLower() == "inactivo")
            .CountAsync();

        var usuariosConPropiedad = await _db.UsuariosPropiedades
            .Select(up => up.IdUsuario)
            .Distinct()
            .CountAsync();

        dto.UsuariosConPropiedades   = usuariosConPropiedad;
        dto.UsuariosSinPropiedades   = dto.TotalUsuarios - usuariosConPropiedad;

        // 5. Navegadores más usados
        dto.NavegadoresUsados = await _db.RegistroAccesos
            .Where(r => !string.IsNullOrEmpty(r.Navegador))
            .GroupBy(r => r.Navegador)
            .Select(g => new ChartDataDto {
                Label = g.Key!.Length > 20 ? g.Key.Substring(0, 20) + "..." : g.Key!,
                Count = g.Count()
            })
            .OrderByDescending(c => c.Count)
            .Take(5)
            .ToListAsync();

        // 6. Páginas más visitadas
        dto.PaginasMasVisitadas = await _db.RegistroAccesos
            .Where(r => r.TipoAccion == "PageView" && !string.IsNullOrEmpty(r.PaginaVisitada))
            .GroupBy(r => r.PaginaVisitada)
            .Select(g => new ChartDataDto {
                Label = g.Key!,
                Count = g.Count()
            })
            .OrderByDescending(c => c.Count)
            .Take(5)
            .ToListAsync();

        // 7. Demografía: Usuarios por Ubicación (Ciudades)
        dto.UsuariosPorUbicacion = await _db.Usuarios
            .Include(u => u.UbicacionNavigation)
            .Where(u => u.UbicacionNavigation != null && !string.IsNullOrEmpty(u.UbicacionNavigation.DireccionFormateada))
            .GroupBy(u => u.UbicacionNavigation!.DireccionFormateada)
            .Select(g => new ChartDataDto {
                Label = g.Key!,
                Count = g.Count()
            })
            .OrderByDescending(c => c.Count)
            .Take(6)
            .ToListAsync();

        // 8. Demografía: Usuarios por Rol
        dto.UsuariosPorRol = await _db.Usuarios
            .Where(u => !string.IsNullOrEmpty(u.Rol))
            .GroupBy(u => u.Rol)
            .Select(g => new ChartDataDto {
                Label = g.Key!,
                Count = g.Count()
            })
            .OrderByDescending(c => c.Count)
            .ToListAsync();

        // 9. Accesos por día — últimos 7 días
        var hace7dias = DateTime.Today.AddDays(-6);
        var accesosPorDia = await _db.RegistroAccesos
            .Where(r => r.FechaAcceso >= hace7dias && r.TipoAccion == "Login")
            .ToListAsync();

        var agrupados = accesosPorDia
            .GroupBy(r => r.FechaAcceso.Date)
            .ToDictionary(g => g.Key, g => g.Count());

        dto.AccesosPorDia = Enumerable.Range(0, 7)
            .Select(i => hace7dias.AddDays(i))
            .Select(fecha => new ChartDataDto {
                Label = fecha.ToString("dd/MM"),
                Count = agrupados.ContainsKey(fecha) ? agrupados[fecha] : 0
            }).ToList();

        // 10. Dispositivos por Tipo
        dto.DispositivosPorTipo = await _db.Dispositivos
            .GroupBy(d => d.Tipo)
            .Select(g => new ChartDataDto {
                Label = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        // 11. Últimos Accesos
        var ultimosAccesos = await _db.RegistroAccesos
            .Include(r => r.IdUsuariosNavigation)
            .OrderByDescending(r => r.FechaAcceso)
            .Take(8)
            .ToListAsync();

        dto.UltimosAccesos = ultimosAccesos.Select(r => new RegistroAccesoResumenDto {
            NombreUsuario = r.IdUsuariosNavigation?.Nombre ?? "N/A",
            Fecha         = r.FechaAcceso,
            TipoAccion    = r.TipoAccion,
            Ip            = r.DireccionIp ?? "—"
        }).ToList();

        // 12. Últimos Soportes — MIGRADO: SoporteResumenDto (deuda técnica eliminada)
        var ultimosSoportes = await _db.Soportes
            .Include(s => s.IdUsuariosNavigation)
            .OrderByDescending(s => s.Fecha)
            .Take(5)
            .ToListAsync();

        dto.UltimosSoportes = ultimosSoportes.Select(s => new SoporteResumenDto {
            Id              = s.Id,
            Tipo            = s.Tipo ?? "—",
            Descripcion     = s.Descripcion ?? "—",
            NombreUsuario   = s.IdUsuariosNavigation?.Nombre ?? "N/A",
            Fecha           = s.Fecha,
            EstaRespondido  = !string.IsNullOrEmpty(s.Respuesta)
        }).ToList();

        return dto;
    }

    // =====================================================================
    //  MÉTODO 2: Métricas de Usuario Individual (vista propia del cliente)
    // =====================================================================
    public async Task<DashboardMetricsDto> GetUserMetricsAsync(int userId)
    {
        var dto = new DashboardMetricsDto();

        var propiedadesIds = await _db.UsuariosPropiedades
            .Where(up => up.IdUsuario == userId)
            .Select(up => up.IdPropiedad)
            .ToListAsync();

        dto.TotalPropiedades = propiedadesIds.Count;

        dto.TotalEspacios = await _db.Espacios
            .Where(e => propiedadesIds.Contains(e.IdPropiedades))
            .CountAsync();

        dto.TotalDispositivos = await _db.Dispositivos
            .Where(d => _db.Espacios
                .Where(e => propiedadesIds.Contains(e.IdPropiedades))
                .Select(e => e.Id)
                .Contains(d.IdEspacio))
            .CountAsync();

        dto.TotalSoportes = await _db.Soportes
            .Where(s => s.IdUsuarios == userId)
            .CountAsync();

        dto.SoportesPendientes = await _db.Soportes
            .Where(s => s.IdUsuarios == userId && string.IsNullOrEmpty(s.Respuesta))
            .CountAsync();

        var ultimosAccesos = await _db.RegistroAccesos
            .Where(r => r.IdUsuarios == userId)
            .OrderByDescending(r => r.FechaAcceso)
            .Take(5)
            .ToListAsync();

        dto.UltimosAccesos = ultimosAccesos.Select(r => new RegistroAccesoResumenDto {
            NombreUsuario = "Tú",
            Fecha         = r.FechaAcceso,
            TipoAccion    = r.TipoAccion,
            Ip            = r.DireccionIp ?? "—"
        }).ToList();

        return dto;
    }

    // =====================================================================
    //  MÉTODO 3: Métricas Detalladas por Cliente (Big Data AJAX)
    // =====================================================================
    public async Task<ClienteMetricasDto?> GetClienteMetricasAsync(int clienteId)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.UbicacionNavigation)
            .FirstOrDefaultAsync(u => u.Id == clienteId);

        if (usuario == null) return null;

        // KPIs de propiedades del cliente
        var propiedadesIds = await _db.UsuariosPropiedades
            .Where(up => up.IdUsuario == clienteId)
            .Select(up => up.IdPropiedad)
            .ToListAsync();

        var espaciosIds = await _db.Espacios
            .Where(e => propiedadesIds.Contains(e.IdPropiedades))
            .Select(e => e.Id)
            .ToListAsync();

        var totalDispositivos = await _db.Dispositivos
            .Where(d => espaciosIds.Contains(d.IdEspacio))
            .CountAsync();

        var totalSoportes = await _db.Soportes
            .Where(s => s.IdUsuarios == clienteId)
            .CountAsync();

        var soportesPendientes = await _db.Soportes
            .Where(s => s.IdUsuarios == clienteId && string.IsNullOrEmpty(s.Respuesta))
            .CountAsync();

        var totalLogins = await _db.RegistroAccesos
            .Where(r => r.IdUsuarios == clienteId && r.TipoAccion == "Login")
            .CountAsync();

        var ultimoAcceso = await _db.RegistroAccesos
            .Where(r => r.IdUsuarios == clienteId)
            .OrderByDescending(r => r.FechaAcceso)
            .Select(r => (DateTime?)r.FechaAcceso)
            .FirstOrDefaultAsync();

        // Timeline de movimientos — derivado de RegistroAccesos
        var registros = await _db.RegistroAccesos
            .Where(r => r.IdUsuarios == clienteId)
            .OrderByDescending(r => r.FechaAcceso)
            .Take(30)
            .ToListAsync();

        var movimientos = registros.Select(r => MapearMovimiento(r)).ToList();

        return new ClienteMetricasDto
        {
            Id                 = usuario.Id,
            Nombre             = usuario.Nombre,
            Correo             = usuario.Correo,
            Rol                = usuario.Rol ?? "—",
            Estado             = usuario.Acesso ?? "—",
            Inicial            = usuario.Nombre.Length > 0 ? usuario.Nombre[0].ToString().ToUpper() : "?",
            TotalPropiedades   = propiedadesIds.Count,
            TotalEspacios      = espaciosIds.Count,
            TotalDispositivos  = totalDispositivos,
            TotalSoportes      = totalSoportes,
            SoportesPendientes = soportesPendientes,
            TotalLogins        = totalLogins,
            UltimoAcceso       = ultimoAcceso,
            Movimientos        = movimientos
        };
    }

    // =====================================================================
    //  MÉTODO 4: Resumen Paginado de Clientes (Big Data Admin Table)
    // =====================================================================
    public async Task<PagedResultDto<ClienteResumenDto>> GetResumenClientesAsync(
        int page, int pageSize, string? busqueda = null)
    {
        // Paso 1: Query base con filtro opcional de búsqueda
        var query = _db.Usuarios.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var texto = busqueda.Trim().ToLower();
            query = query.Where(u =>
                u.Nombre.ToLower().Contains(texto) ||
                u.Correo.ToLower().Contains(texto));
        }

        var totalItems = await query.CountAsync();

        // Paso 2: Obtener IDs de la página actual
        var usuariosPage = await query
            .OrderBy(u => u.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new { u.Id, u.Nombre, u.Correo, u.Rol, u.Acesso })
            .ToListAsync();

        var ids = usuariosPage.Select(u => u.Id).ToList();

        // Paso 3: Cargar métricas agregadas sólo para los usuarios de la página
        var propsPorUsuario = await _db.UsuariosPropiedades
            .Where(up => ids.Contains(up.IdUsuario))
            .GroupBy(up => up.IdUsuario)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        // Espacios: a través de propiedades del usuario
        var propiedadesIds = await _db.UsuariosPropiedades
            .Where(up => ids.Contains(up.IdUsuario))
            .Select(up => new { up.IdUsuario, up.IdPropiedad })
            .ToListAsync();

        var espaciosPorProp = await _db.Espacios
            .Where(e => propiedadesIds.Select(p => p.IdPropiedad).Contains(e.IdPropiedades))
            .Select(e => new { e.Id, e.IdPropiedades })
            .ToListAsync();

        var espaciosPorUsuario = propiedadesIds
            .GroupBy(p => p.IdUsuario)
            .ToDictionary(
                g => g.Key,
                g => espaciosPorProp.Count(e => g.Select(p => p.IdPropiedad).Contains(e.IdPropiedades)));

        // Dispositivos
        var espaciosIds = espaciosPorProp.Select(e => e.Id).ToList();
        var dispositivosPorEspacio = await _db.Dispositivos
            .Where(d => espaciosIds.Contains(d.IdEspacio))
            .Select(d => new { d.Id, d.IdEspacio })
            .ToListAsync();

        var dispositivosPorUsuario = propiedadesIds
            .GroupBy(p => p.IdUsuario)
            .ToDictionary(
                g => g.Key,
                g => {
                    var propIds = g.Select(p => p.IdPropiedad).ToList();
                    var eIds = espaciosPorProp.Where(e => propIds.Contains(e.IdPropiedades)).Select(e => e.Id).ToList();
                    return dispositivosPorEspacio.Count(d => eIds.Contains(d.IdEspacio));
                });

        var soportesPorUsuario = await _db.Soportes
            .Where(s => ids.Contains(s.IdUsuarios))
            .GroupBy(s => s.IdUsuarios)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        var loginsPorUsuario = await _db.RegistroAccesos
            .Where(r => ids.Contains(r.IdUsuarios) && r.TipoAccion == "Login")
            .GroupBy(r => r.IdUsuarios)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        var ultimoAccesoPorUsuario = await _db.RegistroAccesos
            .Where(r => ids.Contains(r.IdUsuarios))
            .GroupBy(r => r.IdUsuarios)
            .Select(g => new { UserId = g.Key, Fecha = g.Max(r => r.FechaAcceso) })
            .ToDictionaryAsync(x => x.UserId, x => (DateTime?)x.Fecha);

        // Paso 4: Proyección al DTO
        var items = usuariosPage.Select(u => new ClienteResumenDto
        {
            Id           = u.Id,
            Nombre       = u.Nombre,
            Correo       = u.Correo,
            Rol          = u.Rol ?? "—",
            Estado       = u.Acesso ?? "—",
            Inicial      = u.Nombre.Length > 0 ? u.Nombre[0].ToString().ToUpper() : "?",
            Propiedades  = propsPorUsuario.GetValueOrDefault(u.Id, 0),
            Espacios     = espaciosPorUsuario.GetValueOrDefault(u.Id, 0),
            Dispositivos = dispositivosPorUsuario.GetValueOrDefault(u.Id, 0),
            Soportes     = soportesPorUsuario.GetValueOrDefault(u.Id, 0),
            Logins       = loginsPorUsuario.GetValueOrDefault(u.Id, 0),
            UltimoAcceso = ultimoAccesoPorUsuario.GetValueOrDefault(u.Id)
        }).ToList();

        return new PagedResultDto<ClienteResumenDto>
        {
            Items      = items,
            TotalItems = totalItems,
            Page       = page,
            PageSize   = pageSize
        };
    }

    // =====================================================================
    //  HELPER PRIVADO: Mapear RegistroAcceso → MovimientoDto
    // =====================================================================
    private static MovimientoDto MapearMovimiento(RegistroAcceso registro)
    {
        var path = registro.PaginaVisitada?.ToLower() ?? "";

        return registro.TipoAccion switch
        {
            "Login" => new MovimientoDto
            {
                Fecha       = registro.FechaAcceso,
                TipoAccion  = "Inicio de Sesión",
                Descripcion = $"Ingresó al sistema desde {registro.DireccionIp ?? "IP desconocida"}",
                Icono       = "fa-sign-in-alt",
                ColorClase  = "text-success"
            },
            "Logout" => new MovimientoDto
            {
                Fecha       = registro.FechaAcceso,
                TipoAccion  = "Cierre de Sesión",
                Descripcion = registro.DuracionSesion.HasValue
                    ? $"Sesión de {Math.Round(registro.DuracionSesion.Value / 60.0, 1)} min"
                    : "Cerró sesión",
                Icono       = "fa-sign-out-alt",
                ColorClase  = "text-secondary"
            },
            "PageView" => MapearPageView(path, registro.FechaAcceso),
            _ => new MovimientoDto
            {
                Fecha       = registro.FechaAcceso,
                TipoAccion  = registro.TipoAccion,
                Descripcion = registro.PaginaVisitada ?? "Acción del sistema",
                Icono       = "fa-circle",
                ColorClase  = "text-muted"
            }
        };
    }

    private static MovimientoDto MapearPageView(string path, DateTime fecha)
    {
        if (path.Contains("propiedades/create"))
            return new MovimientoDto { Fecha = fecha, TipoAccion = "Creó Propiedad", Descripcion = "Registró una nueva propiedad en el sistema", Icono = "fa-building", ColorClase = "text-primary" };

        if (path.Contains("propiedades/edit"))
            return new MovimientoDto { Fecha = fecha, TipoAccion = "Editó Propiedad", Descripcion = "Modificó una propiedad existente", Icono = "fa-edit", ColorClase = "text-warning" };

        if (path.Contains("propiedades/delete"))
            return new MovimientoDto { Fecha = fecha, TipoAccion = "Eliminó Propiedad", Descripcion = "Eliminó una propiedad del sistema", Icono = "fa-trash", ColorClase = "text-danger" };

        if (path.Contains("espacios/create"))
            return new MovimientoDto { Fecha = fecha, TipoAccion = "Creó Espacio", Descripcion = "Registró un nuevo espacio/habitación", Icono = "fa-layer-group", ColorClase = "text-info" };

        if (path.Contains("espacios/edit"))
            return new MovimientoDto { Fecha = fecha, TipoAccion = "Editó Espacio", Descripcion = "Modificó un espacio existente", Icono = "fa-edit", ColorClase = "text-warning" };

        if (path.Contains("dispositivos/create"))
            return new MovimientoDto { Fecha = fecha, TipoAccion = "Agregó Dispositivo", Descripcion = "Registró un nuevo dispositivo inteligente", Icono = "fa-mobile-alt", ColorClase = "text-success" };

        if (path.Contains("dispositivos/edit"))
            return new MovimientoDto { Fecha = fecha, TipoAccion = "Editó Dispositivo", Descripcion = "Modificó un dispositivo inteligente", Icono = "fa-cog", ColorClase = "text-warning" };

        if (path.Contains("usuarios/edit"))
            return new MovimientoDto { Fecha = fecha, TipoAccion = "Asignó Usuario", Descripcion = "Gestionó la asignación de un usuario", Icono = "fa-user-cog", ColorClase = "text-purple" };

        if (path.Contains("soportes"))
            return new MovimientoDto { Fecha = fecha, TipoAccion = "Reporte de Soporte", Descripcion = "Abrió o consultó un ticket de soporte", Icono = "fa-headset", ColorClase = "text-info" };

        if (path.Contains("perfil"))
            return new MovimientoDto { Fecha = fecha, TipoAccion = "Actualizó Perfil", Descripcion = "Modificó su información personal", Icono = "fa-user-edit", ColorClase = "text-primary" };

        return new MovimientoDto
        {
            Fecha       = fecha,
            TipoAccion  = "Navegación",
            Descripcion = $"Visitó: {path}",
            Icono       = "fa-mouse-pointer",
            ColorClase  = "text-muted"
        };
    }
}
