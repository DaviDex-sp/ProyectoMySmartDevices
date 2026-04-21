namespace ProyectoMSD.Modelos.DTOs
{
    public class ComandoDispositivoDto
    {
        public string TargetTopic { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty; // e.g., "ON", "OFF", "SET_TEMP"
        public string Valor { get; set; } = string.Empty;
    }
}
