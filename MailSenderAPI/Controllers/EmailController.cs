
using MailSenderAPI.Models;
using MailSenderAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MailSenderAPI.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

   
        [HttpPost("sendMail")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
        {
            if (emailRequest == null || string.IsNullOrEmpty(emailRequest.To) || string.IsNullOrEmpty(emailRequest.Subject) || string.IsNullOrEmpty(emailRequest.Body))
            {
                return BadRequest("Invalid email data.");
            }

            try
            {
                await _emailService.SendEmailAsync(emailRequest.To, emailRequest.Subject, emailRequest.Body);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
                

            }
        }
    }
}
