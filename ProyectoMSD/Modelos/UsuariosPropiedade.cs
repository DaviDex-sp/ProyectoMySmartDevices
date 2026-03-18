using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProyectoMSD.Modelos;

/// <summary>
/// Tabla de unión N:M entre Usuario y Propiedade.
/// Un usuario puede ser "Propietario", "Residente", "Invitado", etc.
/// Permite que múltiples usuarios compartan una misma propiedad.
/// </summary>
public partial class UsuariosPropiedade
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El usuario es obligatorio")]
    [Column("ID_Usuario")]
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "La propiedad es obligatoria")]
    [Column("ID_Propiedad")]
    public int IdPropiedad { get; set; }

    [StringLength(50)]
    [Display(Name = "Rol en la Propiedad")]
    public string? RolEnPropiedad { get; set; } = "Residente";

    [ValidateNever]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Propiedade IdPropiedadNavigation { get; set; } = null!;
}
