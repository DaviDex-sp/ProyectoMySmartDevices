using System.Collections.Generic;

namespace ProyectoMSD.Modelos.DTOs
{
    public class EspacioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? IdPropiedades { get; set; }
        public string NombrePropiedad { get; set; } = string.Empty;
        
        // Analytics/Stats
        public int TotalDispositivos { get; set; }
        public int DispositivosInactivos { get; set; }
        public int DispositivosActivos { get; set; }
        
        // Sub-elements
        public List<DispositivoDto> Dispositivos { get; set; } = new();
    }
}
