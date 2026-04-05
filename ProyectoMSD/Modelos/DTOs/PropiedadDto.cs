using System.Collections.Generic;

namespace ProyectoMSD.Modelos.DTOs
{
    public class PropiedadDto
    {
        public int Id { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Pisos { get; set; }
        
        // Propietario info
        public string PropietarioNombre { get; set; } = "Usuario no disponible";
        public string PropietarioCorreo { get; set; } = string.Empty;
        public string PropietarioInicial { get; set; } = "U";
        
        // Stats
        public int TotalEspacios { get; set; }
        public int TotalDispositivos { get; set; }
        public double ConsumoEnergeticoSimulado { get; set; }
        
        // UI Helpers
        public string BadgeClass { get; set; } = "bg-secondary";
    }
}
