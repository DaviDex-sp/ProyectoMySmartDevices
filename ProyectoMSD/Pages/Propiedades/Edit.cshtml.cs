using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;

namespace ProyectoMSD.Pages.Propiedades
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ProyectoMSD.Modelos.AppDbContext _context;

        public EditModel(ProyectoMSD.Modelos.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Propiedade Propiedade { get; set; } = default!;

        [BindProperty]
        public decimal? Latitud { get; set; }

        [BindProperty]
        public decimal? Longitud { get; set; }

        [BindProperty]
        public string? DireccionFormateada { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propiedade =  await _context.Propiedades
                .Include(p => p.IdUbicacionNavigation)
                .Include(p => p.UsuariosPropiedades)
                    .ThenInclude(up => up.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (propiedade == null)
            {
                return NotFound();
            }
            Propiedade = propiedade;

            if (propiedade.IdUbicacionNavigation != null)
            {
                Latitud = propiedade.IdUbicacionNavigation.Latitud;
                Longitud = propiedade.IdUbicacionNavigation.Longitud;
                DireccionFormateada = propiedade.IdUbicacionNavigation.DireccionFormateada;
            }

            // Load all users to feed the initial select just in case
            ViewData["IdUsuarios"] = new SelectList(await _context.Usuarios.ToListAsync(), "Id", "Nombre");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload collections and properties before returning
                var propToReload = await _context.Propiedades
                    .Include(p => p.UsuariosPropiedades)
                    .ThenInclude(up => up.IdUsuarioNavigation)
                    .FirstOrDefaultAsync(m => m.Id == Propiedade.Id);
                
                if(propToReload != null) 
                    Propiedade = propToReload;

                return Page();
            }

            var propToUpdate = await _context.Propiedades
                .Include(p => p.IdUbicacionNavigation)
                .FirstOrDefaultAsync(p => p.Id == Propiedade.Id);

            if (propToUpdate == null) return NotFound();

            propToUpdate.Direccion = Propiedade.Direccion;
            propToUpdate.Tipo = Propiedade.Tipo;
            propToUpdate.Pisos = Propiedade.Pisos;

            if (Latitud.HasValue && Longitud.HasValue && !string.IsNullOrEmpty(DireccionFormateada))
            {
                if (propToUpdate.IdUbicacionNavigation == null)
                {
                    propToUpdate.IdUbicacionNavigation = new Ubicacione
                    {
                        FechaCreacion = DateTime.Now
                    };
                }
                propToUpdate.IdUbicacionNavigation.Latitud = Latitud.Value;
                propToUpdate.IdUbicacionNavigation.Longitud = Longitud.Value;
                propToUpdate.IdUbicacionNavigation.DireccionFormateada = DireccionFormateada;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropiedadeExists(Propiedade.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAddUserAsync(int propId, string emailUsuario, string rolEnPropiedad)
        {
            if (string.IsNullOrWhiteSpace(emailUsuario))
            {
                ModelState.AddModelError("", "El correo del usuario es obligatorio.");
                return await OnGetAsync(propId);
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == emailUsuario);
            if (usuario == null)
            {
                ModelState.AddModelError("", "No se encontró ningún usuario con ese correo electrónico.");
                return await OnGetAsync(propId);
            }

            if (await _context.UsuariosPropiedades.AnyAsync(up => up.IdPropiedad == propId && up.IdUsuario == usuario.Id))
            {
                ModelState.AddModelError("", "El usuario ya está asociado a esta propiedad.");
                return await OnGetAsync(propId);
            }

            var nuevaAsociacion = new UsuariosPropiedade
            {
                IdPropiedad = propId,
                IdUsuario = usuario.Id,
                RolEnPropiedad = string.IsNullOrWhiteSpace(rolEnPropiedad) ? "Residente" : rolEnPropiedad
            };

            _context.UsuariosPropiedades.Add(nuevaAsociacion);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = propId });
        }

        public async Task<IActionResult> OnPostRemoveUserAsync(int propId, int userIdToRemove)
        {
            var asociacion = await _context.UsuariosPropiedades
                .FirstOrDefaultAsync(up => up.IdPropiedad == propId && up.IdUsuario == userIdToRemove);

            if (asociacion != null)
            {
                _context.UsuariosPropiedades.Remove(asociacion);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id = propId });
        }

        private bool PropiedadeExists(int id)
        {
            return _context.Propiedades.Any(e => e.Id == id);
        }
    }
}
