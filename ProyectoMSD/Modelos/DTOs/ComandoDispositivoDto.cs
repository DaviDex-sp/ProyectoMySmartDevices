using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos.DTOs
{
    /// <summary>
    /// DTO estructurado para el despacho de comandos a dispositivos IoT via MQTT.
    /// El backend construye el JSON final — el frontend solo declara la intención.
    /// </summary>
    public class ComandoEstructuradoDto
    {
        /// <summary>MAC Address del dispositivo destino. Ej: "24:62:AB:F3:10:A8"</summary>
        [Required]
        public string MacDestino { get; set; } = string.Empty;

        /// <summary>Nombre interno del componente a controlar. Ej: "luz_sala"</summary>
        [Required]
        public string Componente { get; set; } = string.Empty;

        /// <summary>Acción a ejecutar sobre el componente. Ej: "encender", "apagar"</summary>
        [Required]
        public string Comando { get; set; } = string.Empty;
    }
}
