using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos;

public partial class Configuracione
{
    public int Codigo { get; set; }

    public string Idioma { get; set; } = null!;

    public string Alertas { get; set; } = null!;

    public bool Automatizacion { get; set; }

    public string Adaptador { get; set; } = null!;

    public int IdDispositivos { get; set; }

    public int IdUsuarios { get; set; }

    [ValidateNever]
    public virtual Dispositivo IdDispositivosNavigation { get; set; } = null!;
    [ValidateNever]
    public virtual Usuario IdUsuariosNavigation { get; set; } = null!;
}