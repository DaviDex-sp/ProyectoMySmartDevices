using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Interfaces
{
    public interface IPropiedadService
    {
        Task<List<PropiedadDto>> GetPropiedadesAsync(bool isAdmin, int? userId);
        Task<int> GetTotalPropiedadesAsync();
    }
}
