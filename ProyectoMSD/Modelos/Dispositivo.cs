using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoMSD.Modelos;

public partial class Dispositivo
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Debes asignar el dispositivo a un espacio.")]
    [Display(Name = "Espacio Asignado")]
    public int IdEspacio { get; set; }

    [ForeignKey("IdEspacio")]
    public virtual Espacio? Espacio { get; set; } 

    // --- DATOS IOT ---
    [Required(ErrorMessage = "La dirección MAC es obligatoria para la conexión IoT.")]
    [StringLength(50)]
    [Display(Name = "Dirección MAC")]
    public string MAC_Address { get; set; } = null!;

    [Required(ErrorMessage = "El protocolo es obligatorio.")]
    [StringLength(50)]
    public string Protocolo { get; set; } = null!;

    // --- DATOS BÁSICOS ---
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El tipo es obligatorio.")]
    [StringLength(50)]
    public string Tipo { get; set; } = null!;

    [Required(ErrorMessage = "La marca es obligatoria.")]
    [StringLength(50)]
    public string Marca { get; set; } = null!;

    [StringLength(255)]
    public string? Usos { get; set; } 

    [Required(ErrorMessage = "El estado inicial es obligatorio.")]
    [StringLength(30)]
    public string Estado { get; set; } = "Apagado";

    // --- COMPONENTES IoT CONTROLABLES ---
    /// <summary>
    /// JSON serializado de List&lt;ComponenteDto&gt;.
    /// Define los componentes y comandos disponibles para este dispositivo específico.
    /// Ejemplo: [{"nombreInterno":"luz_sala","etiqueta":"Luz Sala","icono":"fas fa-lightbulb","comandos":["encender","apagar"]}]
    /// </summary>
    public string? ComponentesJson { get; set; }

    // --- RELACIONES ACTIVAS ---
    public virtual ICollection<Configuracione> Configuraciones { get; set; } = new List<Configuracione>();
}