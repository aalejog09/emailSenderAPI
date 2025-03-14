namespace MailSenderAPI.Services;

using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using global::MailSenderAPI.Context;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly EncryptionService _encryptionService;

    public EmailService(IConfiguration configuration, ApplicationDbContext context, EncryptionService encryptionService)
    {
        _context = context;
        _configuration = configuration;
        _encryptionService = encryptionService;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            // Eliminar ';' al final si está presente
            to = to.TrimEnd(';');

            // Obtener la configuración SMTP desde la base de datos
            var smtpSettings = await _context.SmtpSettings.OrderBy(s => s.Id).FirstOrDefaultAsync(); //trae siempre el registro mas bajo.

            if (smtpSettings == null)
            {
                throw new Exception("Error obteniendo datos del EmailSender");
            }

            // Crear el mensaje de correo
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("EmailSender API", smtpSettings.FromEmail));

            // Dividir la lista de destinatarios por ';' y agregarlos al mensaje
            var recipients = to.Split(';');
            foreach (var recipient in recipients)
            {
                if (!string.IsNullOrEmpty(recipient))  // Validar que la dirección no esté vacía
                {
                    emailMessage.To.Add(new MailboxAddress("", recipient.Trim()));
                }
            }

            // Configurar el asunto y el cuerpo del mensaje
            emailMessage.Subject = subject;
            string decryptedPassword = _encryptionService.Decrypt(smtpSettings.Password);
            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // Conectar y enviar el correo
            using var client = new SmtpClient();
            await client.ConnectAsync(smtpSettings.Host, smtpSettings.Port, smtpSettings.UseSSL);
            await client.AuthenticateAsync(smtpSettings.Username, decryptedPassword);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
        catch (MailKit.Security.AuthenticationException authEx)
        {
            // Capturar errores de autenticación
            throw new Exception("Authentication failed. Email or password not valid for sender");
        }
        catch (Exception ex)
        {
            // Capturar otros errores
            throw new Exception($"Error al enviar el correo: {ex.Message}");
        }
    }
}