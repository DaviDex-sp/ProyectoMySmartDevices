using Microsoft.AspNetCore.Mvc;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos.DTOs;

namespace ProyectoMSD.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class MqttComandosController : ControllerBase
    {
        private readonly IMqttPublisherService _mqttPublisher;

        public MqttComandosController(IMqttPublisherService mqttPublisher)
        {
            _mqttPublisher = mqttPublisher;
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarComando([FromBody] ComandoDispositivoDto comando)
        {
            if (string.IsNullOrEmpty(comando.TargetTopic) || string.IsNullOrEmpty(comando.Accion))
            {
                return BadRequest(new { success = false, message = "Tópico y Acción son requeridos." });
            }

            // Construir payload básico JSON para el ESP32
            var payload = $"{{\"accion\":\"{comando.Accion}\",\"valor\":\"{comando.Valor}\"}}";
            
            var success = await _mqttPublisher.PublishCommandAsync(comando.TargetTopic, payload);

            if (success)
            {
                return Ok(new { success = true, message = "Comando enviado correctamente." });
            }

            return StatusCode(500, new { success = false, message = "Error enviando el comando al broker." });
        }
    }
}
