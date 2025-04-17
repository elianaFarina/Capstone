using Microsoft.Extensions.Options;
using MuseoMineralogia.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MuseoMineralogia.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(_emailSettings.MailServer, _emailSettings.MailPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password)
            };

            await client.SendMailAsync(
                new MailMessage(_emailSettings.SenderEmail, email, subject, message)
                {
                    IsBodyHtml = true,
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName)
                }
            );
        }
    }
}