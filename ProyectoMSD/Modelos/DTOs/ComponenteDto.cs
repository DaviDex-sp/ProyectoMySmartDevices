using System.Collections.Generic;

namespace ProyectoMSD.Modelos.DTOs
{
    /// <summary>
    /// Representa un componente controlable de un dispositivo IoT.
    /// Los componentes se almacenan como JSON en Dispositivo.ComponentesJson.
    /// </summary>
    public class ComponenteDto
    {
        /// <summary>Identificador interno usado en el payload MQTT. Ej: "luz_sala"</summary>
        public string NombreInterno { get; set; } = string.Empty;

        /// <summary>Nombre visible en la UI. Ej: "Luz de la Sala"</summary>
        public string Etiqueta { get; set; } = string.Empty;

        /// <summary>Clase FontAwesome para el icono. Ej: "fas fa-lightbulb"</summary>
        public string Icono { get; set; } = "fas fa-microchip";

        /// <summary>Lista de comandos disponibles. Ej: ["encender", "apagar"]</summary>
        public List<string> Comandos { get; set; } = new();
    }
}
