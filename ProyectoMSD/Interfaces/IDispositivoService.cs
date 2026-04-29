using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoMSD.Modelos;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Interfaces
{
    public interface IDispositivoService
    {
        Task<List<DispositivoDto>> GetDispositivosAsync();
        Task<int> GetTotalDispositivosAsync();
        Task<bool> ToggleEstadoAsync(int id);
        Task<DispositivoDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(DispositivoDto dto);

        /// <summary>
        /// Persiste un nuevo Dispositivo en la base de datos y genera una
        /// notificacion de configuracion exitosa para el usuario creador.
        /// </summary>
        Task<int> CreateAsync(Dispositivo dispositivo, int idUsuarioCreador);

        /// <summary>
        /// Retorna la lista de componentes controlables configurados para el dispositivo.
        /// Deserializa el campo ComponentesJson de la entidad.
        /// Retorna lista vacia si el dispositivo no existe o no tiene componentes configurados.
        /// </summary>
        Task<List<ComponenteDto>> GetComponentesAsync(int idDispositivo);
    }
}
