using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using QuickRentMyRide.Services.Interfaces;

namespace QuickRentMyRide.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly string smtpServer;
        private readonly int smtpPort;
        private readonly string smtpUser;
        private readonly string smtpPass;

        public EmailService(IConfiguration configuration)
        {
            smtpServer = configuration["Authentication:EmailSettings:SmtpServer"];
            smtpPort = int.Parse(configuration["Authentication:EmailSettings:SmtpPort"]);
            smtpUser = configuration["Authentication:EmailSettings:SmtpUser"];
            smtpPass = configuration["Authentication:EmailSettings:SmtpPass"];
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(smtpUser));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUser, smtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
