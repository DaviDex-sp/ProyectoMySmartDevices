using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Pages.Espacios
{
    public class IndexModel : PageModel
    {
        private readonly IEspacioService _espacioService;
        private readonly IDispositivoService _dispositivoService;

        public IndexModel(IEspacioService espacioService, IDispositivoService dispositivoService)
        {
            _espacioService = espacioService;
            _dispositivoService = dispositivoService;
        }

        public IList<EspacioDto> EspaciosDto { get; set; } = new List<EspacioDto>();
        public int TotalDispositivos { get; set; }
        public List<DispositivoDto> TodosLosDispositivos { get; set; } = new();

        public async Task OnGetAsync()
        {
            EspaciosDto = await _espacioService.GetEspaciosConDispositivosAsync();
            TotalDispositivos = await _dispositivoService.GetTotalDispositivosAsync();
            TodosLosDispositivos = await _dispositivoService.GetDispositivosAsync();
        }
    }
}

