using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos.DTOs
{
    public class CrearNotificacionDto
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(255)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Mensaje { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; } = "info";

        public string? RutaRedireccion { get; set; }
    }
}
