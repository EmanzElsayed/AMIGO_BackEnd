

namespace Amigo.Application.Abstraction.Services.Authentication
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);

    }
}
