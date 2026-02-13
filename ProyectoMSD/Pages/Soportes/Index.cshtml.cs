using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Soportes
{
    [Authorize(Roles = "Admin,Usuario")]

    public class IndexModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public IndexModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        public IList<Soporte> Soporte { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Soporte = await _context.Soportes
                .Include(s => s.IdUsuariosNavigation).ToListAsync();
        }
    }
}
