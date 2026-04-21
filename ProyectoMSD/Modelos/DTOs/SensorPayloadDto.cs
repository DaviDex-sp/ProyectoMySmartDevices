using System;

namespace ProyectoMSD.Modelos.DTOs
{
    public class SensorPayloadDto
    {
        public string DispositivoId { get; set; } = string.Empty;
        public decimal Temperatura { get; set; }
        public decimal Humedad { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
