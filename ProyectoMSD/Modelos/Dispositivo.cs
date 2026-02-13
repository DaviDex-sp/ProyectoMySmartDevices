using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos;

public partial class Dispositivo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Obligatorio")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Obligatorio")]
    public string Tipo { get; set; } = null!;

    [Required(ErrorMessage = "Obligatorio")]
    public string? Usos { get; set; }

    [Required(ErrorMessage = "Obligatorio")]
    public string Marca { get; set; } = null!;

    [Required(ErrorMessage = "Obligatorio")]
    public string Estado { get; set; } = null!;

    public virtual ICollection<Almacenan> Almacenans { get; set; } = new List<Almacenan>();

    public virtual ICollection<Configuracione> Configuraciones { get; set; } = new List<Configuracione>();
}
