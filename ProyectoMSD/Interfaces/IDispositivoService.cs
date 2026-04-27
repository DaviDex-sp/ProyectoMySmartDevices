using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}
