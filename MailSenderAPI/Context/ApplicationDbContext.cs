namespace MailSenderAPI.Context;

using global::MailSenderAPI.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    public DbSet<SmtpSettings> SmtpSettings { get; set; }
}
