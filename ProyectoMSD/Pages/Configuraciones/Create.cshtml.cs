using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Configuraciones
{
    public class CreateModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public CreateModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        public List<SelectListItem> DispositivosList { get; set; } = new();
        public List<SelectListItem> UsuariosList { get; set; } = new();

        [BindProperty]
        public Configuracione Configuracione { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var dispositivos = await _context.Dispositivos.ToListAsync();
            var usuarios = await _context.Usuarios.ToListAsync();

            DispositivosList = dispositivos
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Nombre
                }).ToList();

            UsuariosList = usuarios
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Id} ({(string.IsNullOrWhiteSpace(u.Nombre) ? "SinNombre" : u.Nombre.Split(' ')[0])})"
                }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Recarga las listas si el modelo no es válido, para que el form NO explote
            var dispositivos = await _context.Dispositivos.ToListAsync();
            var usuarios = await _context.Usuarios.ToListAsync();

            DispositivosList = dispositivos
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Nombre
                }).ToList();

            UsuariosList = usuarios
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Id} ({(string.IsNullOrWhiteSpace(u.Nombre) ? "SinNombre" : u.Nombre.Split(' ')[0])})"
                }).ToList();

            if (!ModelState.IsValid)
                return Page();

            _context.Configuraciones.Add(Configuracione);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }

}