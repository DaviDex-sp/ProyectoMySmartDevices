using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;

namespace ProyectoMSD.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _db;

        public UsuarioService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Usuario?> AuthenticateAsync(string correo, string password)
        {
            var hashPassword = HashPassword(password);
            
            return await _db.Usuarios
                .Where(x => x.Correo == correo && x.Clave == hashPassword)
                .FirstOrDefaultAsync();
        }

        public async Task<Usuario> AuthenticateGoogleAsync(string email, string name)
        {
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(x => x.Correo == email);

            if (usuario == null)
            {
                // Crear nuevo usuario con valores por defecto
                usuario = new Usuario
                {
                    Nombre = name ?? "Usuario Google",
                    Correo = email,
                    Clave = "[Autenticado por Google]",
                    Rol = "Usuario",
                    PrefijoTelefono = "+",
                    Telefono = "0",
                    Permisos = "Basico",
                    Acesso = "Activo",
                    Documento = "0"
                };

                _db.Usuarios.Add(usuario);
                await _db.SaveChangesAsync();
            }

            return usuario;
        }

        public async Task RegisterAccessAsync(int userId, string action, string? ip, string? userAgent, string path)
        {
            var registro = new RegistroAcceso
            {
                IdUsuarios = userId,
                FechaAcceso = DateTime.Now,
                TipoAccion = action,
                DireccionIp = ip,
                Navegador = userAgent,
                PaginaVisitada = path
            };
            
            _db.RegistroAccesos.Add(registro);
            await _db.SaveChangesAsync();
        }

        public async Task RegisterLogoutAsync(int userId, string? ip, string? userAgent, string path)
        {
            // Buscar el ultimo login para calcular duracion de sesion
            var ultimoLogin = await _db.RegistroAccesos
                .Where(r => r.IdUsuarios == userId && r.TipoAccion == "Login")
                .OrderByDescending(r => r.FechaAcceso)
                .FirstOrDefaultAsync();

            var registro = new RegistroAcceso
            {
                IdUsuarios = userId,
                FechaAcceso = DateTime.Now,
                TipoAccion = "Logout",
                DireccionIp = ip,
                Navegador = userAgent,
                PaginaVisitada = path,
                DuracionSesion = ultimoLogin != null
                    ? (int)(DateTime.Now - ultimoLogin.FechaAcceso).TotalSeconds
                    : null
            };
            
            _db.RegistroAccesos.Add(registro);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Usuario>> GetAllUsuariosAsync()
        {
            return await _db.Usuarios.Include(u => u.UbicacionNavigation).ToListAsync();
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            return await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task CreateUsuarioAsync(Usuario usuario)
        {
            if (usuario.Clave != null && !usuario.Clave.StartsWith("[Autenticado") && !usuario.Clave.Contains(":")) 
            {
                usuario.Clave = HashPassword(usuario.Clave);
            }
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            _db.Attach(usuario).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteUsuarioAsync(int id)
        {
            var usuario = await _db.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _db.Usuarios.Remove(usuario);
                await _db.SaveChangesAsync();
            }
        }

        public bool EsEmailValido(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public string HashPassword(string password)
        {
            // Hash con PBKDF2 similar a la lógica actual (salt vacio en la imp original)
            byte[] salt = new byte[0];
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public async Task<bool> ExisteCorreoAsync(string correo)
        {
            return await _db.Usuarios.AnyAsync(u => u.Correo.ToLower() == correo.ToLower());
        }

        public async Task<bool> ExisteDocumentoAsync(string documento)
        {
            return await _db.Usuarios.AnyAsync(u => u.Documento == documento);
        }

        public async Task<Usuario?> GetUsuarioPerfilAsync(int userId)
        {
            return await _db.Usuarios
                .Include(u => u.UsuariosPropiedades)
                    .ThenInclude(up => up.IdPropiedadNavigation)
                .Include(u => u.UbicacionNavigation)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> UpdatePerfilAsync(int userId, Usuario datosActualizados, string? latitud, string? longitud, string? direccion, string? nuevaClave)
        {
            var usuarioDb = await _db.Usuarios
                .Include(u => u.UbicacionNavigation)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            if (usuarioDb == null) return false;

            // Actualizar campos personales
            usuarioDb.Nombre = datosActualizados.Nombre;
            usuarioDb.Correo = datosActualizados.Correo;
            usuarioDb.PrefijoTelefono = datosActualizados.PrefijoTelefono;
            usuarioDb.Telefono = datosActualizados.Telefono;

            // Procesar ubicación si se proporcionaron coordenadas válidas
            bool ubicacionActualizada = false;
            if (!string.IsNullOrWhiteSpace(latitud) &&
                !string.IsNullOrWhiteSpace(longitud) &&
                decimal.TryParse(latitud, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
                decimal.TryParse(longitud, NumberStyles.Float, CultureInfo.InvariantCulture, out var lng) &&
                (lat != 0 || lng != 0))
            {
                if (usuarioDb.IdUbicacion.HasValue && usuarioDb.UbicacionNavigation != null)
                {
                    // Actualizar ubicación existente
                    usuarioDb.UbicacionNavigation.Latitud = lat;
                    usuarioDb.UbicacionNavigation.Longitud = lng;
                    usuarioDb.UbicacionNavigation.DireccionFormateada = direccion;
                    _db.Ubicaciones.Update(usuarioDb.UbicacionNavigation);
                }
                else
                {
                    // Crear nueva ubicación y vincularla
                    var nuevaUbicacion = new Ubicacione
                    {
                        Latitud = lat,
                        Longitud = lng,
                        DireccionFormateada = direccion
                    };
                    _db.Ubicaciones.Add(nuevaUbicacion);
                    await _db.SaveChangesAsync();
                    usuarioDb.IdUbicacion = nuevaUbicacion.Id;
                }
                ubicacionActualizada = true;
            }

            // Hashear nueva contraseña si se proporcionó
            bool contrasenaCambiada = false;
            if (!string.IsNullOrWhiteSpace(nuevaClave))
            {
                usuarioDb.Clave = HashPassword(nuevaClave);
                contrasenaCambiada = true;
            }

            // Guardar cambios y registrar notificaciones
            await _db.SaveChangesAsync();

            _db.Notificaciones.Add(new Notificacion
            {
                IdUsuarios = userId,
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
                _db.Notificaciones.Add(new Notificacion
                {
                    IdUsuarios = userId,
                    Titulo = "Contraseña cambiada",
                    Mensaje = "Tu contraseña ha sido cambiada exitosamente.",
                    Tipo = "ContraseñaCambiada",
                    Leida = false,
                    FechaCreacion = DateTime.Now
                });
            }

            await _db.SaveChangesAsync();

            return true;
        }
    }
}
