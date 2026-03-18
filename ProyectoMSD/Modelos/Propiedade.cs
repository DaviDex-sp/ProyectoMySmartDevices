using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos;

public partial class Propiedade
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ingresa la dirección")]
    public string Direccion { get; set; } = null!;

    [Required(ErrorMessage = "Casa o apt")]
    public string Tipo { get; set; } = null!;

    [Required(ErrorMessage = "Ingresa los pisos")]
    public int Pisos { get; set; }

    // --- EL PUENTE FALTANTE HACIA UBICACIONES ---
    [Column("ID_Ubicacion")]
    public int? IdUbicacion { get; set; }

    [ValidateNever]
    [ForeignKey("IdUbicacion")]
    public virtual Ubicacione? IdUbicacionNavigation { get; set; }
    // --------------------------------------------

    public virtual ICollection<Espacio> Espacios { get; set; } = new List<Espacio>();

    public virtual ICollection<UsuariosPropiedade> UsuariosPropiedades { get; set; } = new List<UsuariosPropiedade>();
}