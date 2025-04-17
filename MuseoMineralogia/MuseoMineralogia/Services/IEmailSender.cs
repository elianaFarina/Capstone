using System.Threading.Tasks;

namespace MuseoMineralogia.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}