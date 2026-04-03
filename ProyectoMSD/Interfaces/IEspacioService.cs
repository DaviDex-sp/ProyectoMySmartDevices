using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Interfaces
{
    public interface IEspacioService
    {
        Task<List<EspacioDto>> GetEspaciosConDispositivosAsync();
        Task<int> GetTotalEspaciosAsync();
    }
}
