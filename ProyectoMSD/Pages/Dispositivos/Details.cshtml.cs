using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Dispositivos
{
    public class DetailsModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public DetailsModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        public Dispositivo Dispositivo { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dispositivo = await _context.Dispositivos.FirstOrDefaultAsync(m => m.Id == id);

            if (dispositivo is not null)
            {
                Dispositivo = dispositivo;

                return Page();
            }

            return NotFound();
        }
    }
}
