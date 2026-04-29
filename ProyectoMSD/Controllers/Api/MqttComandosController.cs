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
        private readonly ILogger<MqttComandosController> _logger;

        public MqttComandosController(
            IMqttPublisherService mqttPublisher,
            ILogger<MqttComandosController> logger)
        {
            _mqttPublisher = mqttPublisher;
            _logger        = logger;
        }

        /// <summary>
        /// Recibe un comando estructurado (mac + componente + accion) y lo publica
        /// como JSON al broker MQTT. El frontend nunca conoce el topico MQTT.
        /// POST /api/MqttComandos/comando
        /// </summary>
        [HttpPost("comando")]
        public async Task<IActionResult> EnviarComandoEstructurado([FromBody] ComandoEstructuradoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos incompletos. Se requieren MacDestino, Componente y Comando." });

            _logger.LogInformation(
                "[MqttComandos] Comando recibido | MAC: {Mac} | Componente: {Comp} | Accion: {Cmd}",
                dto.MacDestino, dto.Componente, dto.Comando);

            var success = await _mqttPublisher.PublishStructuredCommandAsync(
                dto.MacDestino, dto.Componente, dto.Comando);

            return success
                ? Ok(new { success = true, message = "Comando enviado correctamente al broker." })
                : StatusCode(500, new { success = false, message = "Error al publicar en el broker MQTT." });
        }
    }
}
