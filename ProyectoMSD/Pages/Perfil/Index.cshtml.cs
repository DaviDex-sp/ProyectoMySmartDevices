using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Modelos;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Globalization;

namespace ProyectoMSD.Pages.Perfil
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;

        [BindProperty]
        public string? NuevaClave { get; set; }

        // Campos de Ubicación
        [BindProperty]
        public string? UbicacionLatitud { get; set; }

        [BindProperty]
        public string? UbicacionLongitud { get; set; }

        [BindProperty]
        public string? UbicacionDireccion { get; set; }

        public string? MensajeExito { get; set; }
        public bool MostrarAvisoGoogle { get; set; }

        public async Task<IActionResult> OnGetAsync(bool completarPerfil = false)
        {
            var userId = ObtenerUsuarioId();
            if (userId == null) return RedirectToPage("/Index");

            var usuario = await _context.Usuarios
                .Include(u => u.UsuariosPropiedades)
                    .ThenInclude(up => up.IdPropiedadNavigation)
                .Include(u => u.UbicacionNavigation)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null) return NotFound();

            Usuario = usuario;
            MostrarAvisoGoogle = completarPerfil;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = ObtenerUsuarioId();
            if (userId == null) return RedirectToPage("/Index");

            var usuarioDb = await _context.Usuarios
                .Include(u => u.UbicacionNavigation)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (usuarioDb == null) return NotFound();

            // Actualizar campos personales
            usuarioDb.Nombre = Usuario.Nombre;
            usuarioDb.Correo = Usuario.Correo;
            usuarioDb.PrefijoTelefono = Usuario.PrefijoTelefono;
            usuarioDb.Telefono = Usuario.Telefono;

            // Procesar ubicación si se proporcionaron coordenadas válidas
            bool ubicacionActualizada = false;
            if (!string.IsNullOrWhiteSpace(UbicacionLatitud) &&
                !string.IsNullOrWhiteSpace(UbicacionLongitud) &&
                decimal.TryParse(UbicacionLatitud, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
                decimal.TryParse(UbicacionLongitud, NumberStyles.Float, CultureInfo.InvariantCulture, out var lng) &&
                (lat != 0 || lng != 0))
            {
                if (usuarioDb.IdUbicacion.HasValue && usuarioDb.UbicacionNavigation != null)
                {
                    // Actualizar ubicación existente
                    usuarioDb.UbicacionNavigation.Latitud = lat;
                    usuarioDb.UbicacionNavigation.Longitud = lng;
                    usuarioDb.UbicacionNavigation.DireccionFormateada = UbicacionDireccion;
                    _context.Ubicaciones.Update(usuarioDb.UbicacionNavigation);
                }
                else
                {
                    // Crear nueva ubicación y vincularla
                    var nuevaUbicacion = new Ubicacione
                    {
                        Latitud = lat,
                        Longitud = lng,
                        DireccionFormateada = UbicacionDireccion
                    };
                    _context.Ubicaciones.Add(nuevaUbicacion);
                    await _context.SaveChangesAsync();
                    usuarioDb.IdUbicacion = nuevaUbicacion.Id;
                }
                ubicacionActualizada = true;
            }

            // Hashear nueva contraseña si se proporcionó
            bool contrasenaCambiada = false;
            if (!string.IsNullOrWhiteSpace(NuevaClave))
            {
                byte[] salt = new byte[0];
                var pbkdf2 = new Rfc2898DeriveBytes(NuevaClave, salt, 100_000, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(32);
                usuarioDb.Clave = Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
                contrasenaCambiada = true;
            }

            try
            {
                await _context.SaveChangesAsync();

                _context.Notificaciones.Add(new Notificacion
                {
                    IdUsuarios = userId.Value,
                    Titulo = "Perfil actualizado",
                    Mensaje = ubicacionActualizada
                        ? "Has actualizado tu información personal y ubicación exitosamente."
                        : "Has actualizado tu información personal exitosamente.",
                    Tipo = "PerfilActualizado",
                    Leida = false,
                    FechaCreacion = DateTime.Now
                });

                if (contrasenaCambiada)
                {
                    _context.Notificaciones.Add(new Notificacion
                    {
                        IdUsuarios = userId.Value,
                        Titulo = "Contraseña cambiada",
                        Mensaje = "Tu contraseña ha sido cambiada exitosamente.",
                        Tipo = "ContraseñaCambiada",
                        Leida = false,
                        FechaCreacion = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
                MensajeExito = "Perfil actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usuarios.Any(e => e.Id == userId))
                    return NotFound();
                throw;
            }

            // Recargar datos actualizados para la vista
            Usuario = await _context.Usuarios
                .Include(u => u.UsuariosPropiedades)
                    .ThenInclude(up => up.IdPropiedadNavigation)
                .Include(u => u.UbicacionNavigation)
                .FirstAsync(u => u.Id == userId);

            return Page();
        }

        private int? ObtenerUsuarioId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            return null;
        }
    }
}
