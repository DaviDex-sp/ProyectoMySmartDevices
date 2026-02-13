using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos;

public partial class Espacio
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Obligatorio")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Obligatorio")]
    public string Ubicacion { get; set; } = null!;

    [Required(ErrorMessage = "Obligatorio")]
    public int Señal { get; set; }

    [Required(ErrorMessage = "Obligatorio")]
    public string Permisos { get; set; } = null!;

    [Required(ErrorMessage = "Ingresa la propiedad")]
    public int IdPropiedades { get; set; }

    public virtual ICollection<Almacenan> Almacenans { get; set; } = new List<Almacenan>();
    [ValidateNever]
    public virtual Propiedade IdPropiedadesNavigation { get; set; } = null!;
}
