using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos;

public partial class Propiedade
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ingresa la dirreccion")]
    public string Direccion { get; set; } = null!;

    [Required(ErrorMessage = "Casa o apt")]
    public string Tipo { get; set; } = null!;

    [Required(ErrorMessage = "Ingresa los pisos")]
    public int Pisos { get; set; }

    [Required(ErrorMessage = "Ingrese el usuario")]
    public int IdUsuarios { get; set; }

    public virtual ICollection<Espacio> Espacios { get; set; } = new List<Espacio>();
    [ValidateNever]
    public virtual Usuario IdUsuariosNavigation { get; set; } = null!;
}