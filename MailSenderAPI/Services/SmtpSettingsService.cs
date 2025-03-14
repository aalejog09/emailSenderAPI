using MailSenderAPI.Context;
using MailSenderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MailSenderAPI.Services
{
    public class SmtpSettingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly EncryptionService _encryptionService;

        public SmtpSettingsService(ApplicationDbContext context, EncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }


        // Actualizar un registro existente
        public async Task<SmtpSettings> UpdateAsync(int id, SmtpSettings smtpSettings)
        {
            var existingSmtpSettings = await _context.SmtpSettings.FirstOrDefaultAsync(s => s.Id == id);
            if (existingSmtpSettings == null)
                return null;

            string EncryptedNewPassword = _encryptionService.Encrypt(smtpSettings.Password);

            // Actualizar los campos del registro
            existingSmtpSettings.Host = smtpSettings.Host;
            existingSmtpSettings.Port = smtpSettings.Port;
            existingSmtpSettings.Username = smtpSettings.Username;
            existingSmtpSettings.Password = EncryptedNewPassword;
            existingSmtpSettings.UseSSL = smtpSettings.UseSSL;
            existingSmtpSettings.FromEmail = smtpSettings.FromEmail;

            await _context.SaveChangesAsync();
            return existingSmtpSettings;
        }



        public async Task<List<SmtpSettings>> GetAllSmtpSettingsAsync()
        {
            return await _context.SmtpSettings.ToListAsync();
        }

        // Método para guardar los SMTP settings con la clave cifrada
        public async Task<SmtpSettings> CreateAsync(SmtpSettings smtpSettings)
        {
            if (smtpSettings == null)
            {
                throw new ArgumentNullException(nameof(smtpSettings));
            }

            // Cifrar la contraseña
            smtpSettings.Password = _encryptionService.Encrypt(smtpSettings.Password);

            _context.SmtpSettings.Add(smtpSettings); 
            await _context.SaveChangesAsync();  

            return smtpSettings;  // Devolvemos el objeto con el ID autogenerado
        }

        public async Task<SmtpSettings> GetByIdAsync(int id)
        {
            // Verifica que la búsqueda sea correcta
            return await _context.SmtpSettings.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Obtener el registro a eliminar por el ID
            var smtpSettings = await _context.SmtpSettings.FirstOrDefaultAsync(s => s.Id == id);
            if (smtpSettings == null)
            {
                return false; // Si no se encuentra el registro, devolver false
            }

            // Eliminar el registro de la base de datos
            _context.SmtpSettings.Remove(smtpSettings);
            await _context.SaveChangesAsync();

            return true; // El registro fue eliminado correctamente
        }


    }
}