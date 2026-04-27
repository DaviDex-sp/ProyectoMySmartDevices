using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos.DTOs;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Dispositivos
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IDispositivoService _dispositivoService;
        private readonly IEspacioService _espacioService;

        public EditModel(IDispositivoService dispositivoService, IEspacioService espacioService)
        {
            _dispositivoService = dispositivoService;
            _espacioService = espacioService;
        }

        [BindProperty]
        public DispositivoDto Dispositivo { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dispositivo = await _dispositivoService.GetByIdAsync(id.Value);
            if (dispositivo == null)
            {
                return NotFound();
            }

            Dispositivo = dispositivo;
            await CargarEspaciosAsync(Dispositivo.IdEspacio);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarEspaciosAsync(Dispositivo.IdEspacio);
                return Page();
            }

            var success = await _dispositivoService.UpdateAsync(Dispositivo);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "No se pudo actualizar el dispositivo. Inténtalo de nuevo.");
                await CargarEspaciosAsync(Dispositivo.IdEspacio);
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task CargarEspaciosAsync(int selectedId = 0)
        {
            var espacios = await _espacioService.GetEspaciosConDispositivosAsync();
            ViewData["IdEspacio"] = new SelectList(espacios, "Id", "Nombre", selectedId);
        }
    }
}
