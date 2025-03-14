using MailSenderAPI.Models;
using MailSenderAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MailSenderAPI.Controllers
{
    [Route("api/email/settings")]
    [ApiController]
    public class SmtpSettingsController : ControllerBase
    {
        private readonly SmtpSettingsService _smtpSettingsService;

        public SmtpSettingsController(SmtpSettingsService smtpSettingsService)
        {
            _smtpSettingsService = smtpSettingsService;
        }


        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SmtpSettings smtpSettings)
        {
            if (smtpSettings == null || id != smtpSettings.Id)
            {
                return BadRequest();
            }

            var updatedSmtpSettings = await _smtpSettingsService.UpdateAsync(id, smtpSettings);
            if (updatedSmtpSettings == null)
            {
                return NotFound();
            }

            return Ok(updatedSmtpSettings);
        }


        [HttpGet("list")]
        public async Task<IActionResult> GetSmtpSettings()
        {
            try
            {
                var smtpSettingsList = await _smtpSettingsService.GetAllSmtpSettingsAsync();

                // Si la lista está vacía
                if (smtpSettingsList == null || smtpSettingsList.Count == 0)
                {
                    return NotFound("No SMTP settings found.");
                }

                return Ok(smtpSettingsList);
            }
            catch (Exception ex)
            {
                // Si ocurre algún error en el proceso, retornamos un error 500
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // Ruta para crear SMTP settings con la contraseña cifrada
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SmtpSettings smtpSettings)
        {
            try
            {
                if (smtpSettings == null)
                {
                    return BadRequest("SMTP settings are required.");
                }

                // Llamamos al servicio para crear el SMTP settings con la contraseña cifrada
                var createdSmtpSettings = await _smtpSettingsService.CreateAsync(smtpSettings);
                return CreatedAtAction(nameof(GetById), new { id = createdSmtpSettings.Id }, createdSmtpSettings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // Ruta para obtener un solo SMTP setting por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var smtpSettings = await _smtpSettingsService.GetByIdAsync(id);

                if (smtpSettings == null)
                {
                    return NotFound(new { message = "SMTP settings not found." });
                }

                return Ok(smtpSettings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _smtpSettingsService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "SMTP settings not found." });
                }

                return Ok(new { message = "SMTP settings deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }


    }


}
