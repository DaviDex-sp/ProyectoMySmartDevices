using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos;

public partial class Soporte
{
    public int Id { get; set; }

    public DateOnly Fecha { get; set; }

    [Required(ErrorMessage = "Describa su situacion")]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage = "Ingresa el Tipo de consulta")]
    public string Tipo { get; set; } = null!;

    public string Respuesta { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese el Usuario")]
    public int IdUsuarios { get; set; }

    [ValidateNever]
    public virtual Usuario IdUsuariosNavigation { get; set; } = null!;
}