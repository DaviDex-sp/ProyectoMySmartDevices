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
    [MaxLength(2000, ErrorMessage = "La descripción no puede superar 2000 caracteres.")]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage = "Ingresa el Tipo de consulta")]
    [MaxLength(50, ErrorMessage = "El tipo no puede superar 50 caracteres.")]
    public string Tipo { get; set; } = null!;

    [MaxLength(3000, ErrorMessage = "La respuesta no puede superar 3000 caracteres.")]
    public string Respuesta { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese el Usuario")]
    public int IdUsuarios { get; set; }

    [ValidateNever]
    public virtual Usuario IdUsuariosNavigation { get; set; } = null!;
}