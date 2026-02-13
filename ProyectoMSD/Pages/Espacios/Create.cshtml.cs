using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoMSD.Pages.Espacios
{
    public class CreateModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public CreateModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Espacio Espacio { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadPropiedadesDropdown();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("===== DEBUG ESPACIOS =====");
            Console.WriteLine($"Nombre: {Espacio.Nombre}, Ubicación: {Espacio.Ubicacion}, Señal: {Espacio.Señal}, Permisos: {Espacio.Permisos}, PropiedadID: {Espacio.IdPropiedades}");

            if (Espacio.IdPropiedades <= 0)
            {
                ModelState.AddModelError("Espacio.IdPropiedades", "Debe seleccionar una propiedad válida");
            }
            else
            {
                var propiedadExists = await _context.Propiedades.AnyAsync(p => p.Id == Espacio.IdPropiedades);
                if (!propiedadExists)
                {
                    ModelState.AddModelError("Espacio.IdPropiedades", "La propiedad seleccionada no existe");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadPropiedadesDropdown();
                return Page();
            }

            try
            {
                _context.Espacios.Add(Espacio);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message?.Contains("FOREIGN KEY") == true)
                {
                    ModelState.AddModelError("Espacio.IdPropiedades", "Error: La propiedad seleccionada no es válida");
                }
                else
                {
                    ModelState.AddModelError("", $"Error al guardar: {ex.InnerException?.Message ?? ex.Message}");
                }

                await LoadPropiedadesDropdown();
                return Page();
            }
        }

        private async Task LoadPropiedadesDropdown()
        {
            var propiedades = await _context.Propiedades
                .OrderBy(p => p.Direccion)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Direccion} ({p.Tipo})"
                })
                .ToListAsync();

            ViewData["IdPropiedades"] = new SelectList(propiedades, "Value", "Text");
            Console.WriteLine($"Propiedades cargadas: {propiedades.Count}");
        }
    }
}
