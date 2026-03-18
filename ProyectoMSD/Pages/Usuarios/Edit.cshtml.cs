using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ProyectoMSD.Interfaces;

namespace ProyectoMSD.Pages.Usuarios
{
    public class EditModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly AppDbContext _context;

        public EditModel(IUsuarioService usuarioService, AppDbContext context)
        {
            _usuarioService = usuarioService;
            _context = context;
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;

        [BindProperty]
        public string? DireccionFormateada { get; set; }

        [BindProperty]
        public decimal? Latitud { get; set; }

        [BindProperty]
        public decimal? Longitud { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario =  await _context.Usuarios
                .Include(u => u.UbicacionNavigation)
                .FirstOrDefaultAsync(u => u.Id == id.Value);

            if (usuario == null)
            {
                return NotFound();
            }
            Usuario = usuario;

            if (usuario.UbicacionNavigation != null)
            {
                DireccionFormateada = usuario.UbicacionNavigation.DireccionFormateada;
                Latitud = usuario.UbicacionNavigation.Latitud;
                Longitud = usuario.UbicacionNavigation.Longitud;
            }
            
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var userToUpdate = await _context.Usuarios
                .Include(u => u.UbicacionNavigation)
                .FirstOrDefaultAsync(u => u.Id == Usuario.Id);

            if (userToUpdate == null) return NotFound();

            // Actualizar campos escalares
            userToUpdate.Nombre = Usuario.Nombre;
            userToUpdate.Correo = Usuario.Correo;
            userToUpdate.Rol = Usuario.Rol;
            userToUpdate.PrefijoTelefono = Usuario.PrefijoTelefono;
            userToUpdate.Telefono = Usuario.Telefono;
            userToUpdate.Permisos = Usuario.Permisos;
            userToUpdate.Documento = Usuario.Documento;
            userToUpdate.Rut = Usuario.Rut;
            userToUpdate.Acesso = Usuario.Acesso;

            // Si la clave se envía nueva (y no es el hash actual), la actualizamos
            if (!string.IsNullOrEmpty(Usuario.Clave) && !Usuario.Clave.Contains(":")) 
            {
                userToUpdate.Clave = _usuarioService.HashPassword(Usuario.Clave);
            }

            // Actualizar el mapa / ubicación
            if (!string.IsNullOrEmpty(DireccionFormateada) && Latitud.HasValue && Longitud.HasValue)
            {
                if (userToUpdate.UbicacionNavigation == null)
                {
                    userToUpdate.UbicacionNavigation = new Ubicacione();
                }
                userToUpdate.UbicacionNavigation.DireccionFormateada = DireccionFormateada;
                userToUpdate.UbicacionNavigation.Latitud = Latitud.Value;
                userToUpdate.UbicacionNavigation.Longitud = Longitud.Value;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Usuarios.AnyAsync(e => e.Id == Usuario.Id);
                if (!exists)
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
    }
}
