using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;
using ProyectoMSD.Modelos.DTOs;
using System.Text.RegularExpressions;

namespace ProyectoMSD.Services
{
    /// <summary>
    /// Implementación del servicio de Soporte.
    /// Centraliza sanitización de entradas, whitelist de tipos y acceso a datos.
    /// </summary>
    public class SoporteService : ISoporteService
    {
        private readonly AppDbContext _context;

        // Whitelist de valores permitidos para el campo Tipo (ambos formularios)
        private static readonly HashSet<string> _tiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
        {
            "consulta", "problema", "sugerencia", "instalacion", "configuracion", "facturacion",
            "Técnico", "Consulta", "Incidencia", "Solicitud", "Mantenimiento"
        };

        public SoporteService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Soporte?> ObtenerPorIdAsync(int id, bool incluirUsuario = false)
        {
            var query = _context.Soportes.AsQueryable();
            if (incluirUsuario)
                query = query.Include(s => s.IdUsuariosNavigation);
            return await query.FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <inheritdoc />
        public async Task<bool> CrearAsync(CrearSoporteDto dto, int usuarioId)
        {
            if (!EsTipoValido(dto.Tipo))
                return false;

            var soporte = new Soporte
            {
                Fecha       = DateOnly.FromDateTime(DateTime.UtcNow),
                IdUsuarios  = usuarioId,
                Respuesta   = "Pendiente",
                Tipo        = SanitizarTexto(dto.Tipo, 50),
                Descripcion = SanitizarTexto(dto.Descripcion, 2000)
            };

            _context.Soportes.Add(soporte);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> EditarAsync(EditarSoporteDto dto)
        {
            var soporte = await _context.Soportes.FindAsync(dto.Id);
            if (soporte == null)
                return false;

            if (!EsTipoValido(dto.Tipo))
                return false;

            // Asignación explícita campo a campo — NUNCA Attach completo (previene over-posting)
            soporte.Fecha       = dto.Fecha;
            soporte.Tipo        = SanitizarTexto(dto.Tipo, 50);
            soporte.Descripcion = SanitizarTexto(dto.Descripcion, 2000);
            soporte.Respuesta   = SanitizarTexto(dto.Respuesta, 3000);
            soporte.IdUsuarios  = dto.IdUsuarios;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> ResponderAsync(int soporteId, string respuesta)
        {
            var soporte = await _context.Soportes.FindAsync(soporteId);
            if (soporte == null)
                return false;

            soporte.Respuesta = SanitizarTexto(respuesta, 3000);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public string SanitizarTexto(string? input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 1. Strip de tags HTML / scripts
            var sinHtml = Regex.Replace(input, @"<[^>]*>", string.Empty);

            // 2. Eliminar caracteres de control peligrosos (excepto \t \n \r)
            var sinControl = Regex.Replace(sinHtml, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", string.Empty);

            // 3. Colapsar espacios múltiples
            var colapsado = Regex.Replace(sinControl, @" {2,}", " ").Trim();

            // 4. Truncar al límite autorizado
            return colapsado.Length > maxLength
                ? colapsado[..maxLength]
                : colapsado;
        }

        /// <inheritdoc />
        public bool EsTipoValido(string tipo)
            => !string.IsNullOrWhiteSpace(tipo) && _tiposPermitidos.Contains(tipo);
    }
}
