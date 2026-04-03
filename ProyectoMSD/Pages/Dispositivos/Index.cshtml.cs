using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Pages.Dispositivos
{
    public class IndexModel : PageModel
    {
        private readonly IDispositivoService _dispositivoService;

        public IndexModel(IDispositivoService dispositivoService)
        {
            _dispositivoService = dispositivoService;
        }

        public IList<DispositivoDto> DispositivosDto { get; set; } = new List<DispositivoDto>();

        public async Task OnGetAsync()
        {
            DispositivosDto = await _dispositivoService.GetDispositivosAsync();
        }

        public async Task<IActionResult> OnPostToggleEstadoAsync(int id)
        {
            var result = await _dispositivoService.ToggleEstadoAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToPage();
        }
    }
}


