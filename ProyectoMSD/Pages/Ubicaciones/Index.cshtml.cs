using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Ubicaciones
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Ubicacione> Ubicacion { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Ubicacion = await _context.Ubicaciones.ToListAsync();
        }
    }
}
