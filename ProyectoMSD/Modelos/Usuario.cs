using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoMSD.Modelos;

public partial class Usuario
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Se necesita un correo")]
    [StringLength(200)]
    public string Correo { get; set; } = null!;

    [StringLength(50)]
    public string? Clave { get; set; } // Opcional para logins externos

    [Required(ErrorMessage = "Especifica un Rol")]
    public string Rol { get; set; } = null!;

    public string? PrefijoTelefono { get; set; }

    public string? Telefono { get; set; }

    // --- GEOLOCALIZACIÓN (EL PUENTE CRÍTICO) ---
    [Display(Name = "Ubicación")]
    public int? IdUbicacion { get; set; } // Debe llamarse exactamente así

    [ForeignKey("IdUbicacion")]
    public virtual Ubicacione? UbicacionNavigation { get; set; }

    // --- OTROS DATOS ---
    public string? Permisos { get; set; }
    public string? Documento { get; set; }
    public string? Rut { get; set; }
    public string? Acesso { get; set; }
    public string? FotoPerfil_URL { get; set; }

    // --- COLECCIONES (Relaciones) ---
    public virtual ICollection<Configuracione> Configuraciones { get; set; } = new List<Configuracione>();
    public virtual ICollection<Soporte> Soportes { get; set; } = new List<Soporte>();
    public virtual ICollection<RegistroAcceso> RegistroAccesos { get; set; } = new List<RegistroAcceso>();
    public virtual ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
    public virtual ICollection<UsuariosPropiedade> UsuariosPropiedades { get; set; } = new List<UsuariosPropiedade>();
}