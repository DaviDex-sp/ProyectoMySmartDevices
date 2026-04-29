using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Dispositivos
{
    /// <summary>
    /// PageModel para la pagina de detalles de un dispositivo IoT.
    /// Carga el dispositivo y sus componentes controlables via IDispositivoService.
    /// No accede directamente a AppDbContext (arquitectura limpia).
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly IDispositivoService _dispositivoService;

        public DetailsModel(IDispositivoService dispositivoService)
        {
            _dispositivoService = dispositivoService;
        }

        public DispositivoDto? Dispositivo { get; set; }
        public List<ComponenteDto> Componentes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Dispositivo = await _dispositivoService.GetByIdAsync(id.Value);
            if (Dispositivo == null) return NotFound();

            Componentes = await _dispositivoService.GetComponentesAsync(id.Value);
            return Page();
        }
    }
}
