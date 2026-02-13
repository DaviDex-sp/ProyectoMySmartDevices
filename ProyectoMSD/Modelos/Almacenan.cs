using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos;

public partial class Almacenan
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Obligatorio")]
    public bool Estado { get; set; }

    [Required(ErrorMessage = "Ingrese el Espacio")]
    public int IdEspacios { get; set; }

    [Required(ErrorMessage = "Ingresa el Dispositivo")]
    public int IdDispositivos { get; set; }

    public virtual Dispositivo IdDispositivosNavigation { get; set; } = null!;

    public virtual Espacio IdEspaciosNavigation { get; set; } = null!;
}
