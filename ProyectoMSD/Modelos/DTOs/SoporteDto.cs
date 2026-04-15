using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos.DTOs
{
    /// <summary>
    /// DTO de entrada para la creación de un ticket de soporte.
    /// Solo expone los campos que el usuario puede enviar; previene over-posting.
    /// </summary>
    public class CrearSoporteDto
    {
        [Required(ErrorMessage = "El tipo de consulta es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El tipo no puede superar 50 caracteres.")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [MaxLength(2000, ErrorMessage = "La descripción no puede superar 2000 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de entrada para la edición de un ticket (Usuario y Admin).
    /// Campos acotados explícitamente; IdUsuarios solo editable por Admin a nivel negocio.
    /// </summary>
    public class EditarSoporteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateOnly Fecha { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El tipo no puede superar 50 caracteres.")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [MaxLength(2000, ErrorMessage = "La descripción no puede superar 2000 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [MaxLength(3000, ErrorMessage = "La respuesta no puede superar 3000 caracteres.")]
        public string? Respuesta { get; set; }

        [Required(ErrorMessage = "El usuario responsable es obligatorio.")]
        public int IdUsuarios { get; set; }
    }
}
