using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Pages.Propiedades
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IPropiedadService _propiedadService;

        public IndexModel(IPropiedadService propiedadService)
        {
            _propiedadService = propiedadService;
        }

        public IList<PropiedadDto> PropiedadesDto { get; set; } = new List<PropiedadDto>();
        public int TotalPropiedades { get; set; }

        public async Task OnGetAsync()
        {
            var isAdmin = User.IsInRole("Admin");
            int? userId = null;
            
            if (!isAdmin)
            {
                var userIdString = User.FindFirst("UserId")?.Value;
                if (int.TryParse(userIdString, out int parsedId))
                {
                    userId = parsedId;
                }
            }

            PropiedadesDto = await _propiedadService.GetPropiedadesAsync(isAdmin, userId);
            TotalPropiedades = await _propiedadService.GetTotalPropiedadesAsync();
        }
    }
}
