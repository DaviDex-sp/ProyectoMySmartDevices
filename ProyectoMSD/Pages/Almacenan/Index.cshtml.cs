using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Pages.Almacenan
{
    public class IndexModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public IndexModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        public IList<ProyectoMSD.Modelos.Almacenan> Almacenan { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Almacenan = await _context.Almacenans
                .Include(a => a.IdDispositivosNavigation)
                .Include(a => a.IdEspaciosNavigation).ToListAsync();
        }
    }
}
