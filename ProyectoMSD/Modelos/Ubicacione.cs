using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProyectoMSD.Modelos;

public partial class Ubicacione
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required(ErrorMessage = "La latitud es obligatoria")]
    [Range(-90, 90, ErrorMessage = "Latitud debe estar entre -90 y 90")]
    [Column("Latitud", TypeName = "decimal(18,12)")]
    public decimal Latitud { get; set; }

    [Required(ErrorMessage = "La longitud es obligatoria")]
    [Column("Longitud", TypeName = "decimal(18,12)")]
    [Range(-180, 180, ErrorMessage = "Longitud debe estar entre -180 y 180")]
    public decimal Longitud { get; set; }

    [StringLength(255)]
    [Display(Name = "Dirección Formateada")]
    // Sincronizamos con el nombre real en MySQL (Direccion_Formateada)
    [Column("Direccion_Formateada")] 
    public string? DireccionFormateada { get; set; }

    [Display(Name = "Fecha de Registro")]
    [Column("FechaCreacion")] // Asegúrate que coincida con tu BD
    public DateTime? FechaCreacion { get; set; }

    [ValidateNever]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
