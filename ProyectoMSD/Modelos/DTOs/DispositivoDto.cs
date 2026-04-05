using System.Collections.Generic;

namespace ProyectoMSD.Modelos.DTOs
{
    public class DispositivoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Usos { get; set; } = string.Empty;
        
        // UI Helpers
        public bool IsActive { get; set; }
        public string IconClass { get; set; } = "fas fa-microchip";
        public string BadgeClass { get; set; } = "bg-secondary";
        public string TextColorClass { get; set; } = "text-muted";
    }
}
