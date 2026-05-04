


using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Amigo.Application.Services
{
    public class EmailService(IConfiguration _configuration) : IEmailService
    {
        private readonly string Host = _configuration["EmailSettings:Host"];

        private readonly string PortValue = _configuration["EmailSettings:Port"];

        private readonly string SenderName = _configuration["EmailSettings:SenderName"];

        private readonly string SenderEmail = _configuration["EmailSettings:SenderEmail"];

        private readonly string Password = _configuration["EmailSettings:Password"];


        public async Task SendEmailAsync(string to, string subject, string body, byte[]? qrImage = null)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(SenderName, SenderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            if (qrImage != null)
            {
                var image = builder.LinkedResources.Add("qr.png", qrImage);
                image.ContentId = "qrCode"; 
            }

            email.Body = builder.ToMessageBody();
            int.TryParse(PortValue, out int Port);
            using var smtp = new SmtpClient();
            try
            {
               await smtp.ConnectAsync(Host, Port, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(SenderEmail, Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }
    }
}

