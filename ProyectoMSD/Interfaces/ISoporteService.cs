using ProyectoMSD.Modelos;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Interfaces
{
    /// <summary>
    /// Contrato del servicio de Soporte.
    /// Centraliza validación, sanitización y operaciones de persistencia.
    /// </summary>
    public interface ISoporteService
    {
        Task<Soporte?> ObtenerPorIdAsync(int id, bool incluirUsuario = false);
        Task<bool> CrearAsync(CrearSoporteDto dto, int usuarioId);
        Task<bool> EditarAsync(EditarSoporteDto dto);
        Task<bool> ResponderAsync(int soporteId, string respuesta);

        /// <summary>
        /// Elimina tags HTML, caracteres de control y trunca al límite dado.
        /// </summary>
        string SanitizarTexto(string? input, int maxLength);

        /// <summary>
        /// Verifica que el tipo de soporte pertenezca a la whitelist permitida.
        /// </summary>
        bool EsTipoValido(string tipo);
    }
}
