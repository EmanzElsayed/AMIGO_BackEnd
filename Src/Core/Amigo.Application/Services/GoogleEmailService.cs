


namespace Amigo.Application.Services
{
    public class GoogleEmailService(IConfiguration _configuration) : IEmailService
    {
        private readonly string SmtpServer = _configuration["EmailSettings:SmtpServer"];

        private readonly int Port = 587;

        private readonly string SenderName = _configuration["EmailSettings:SenderName"];

        private readonly string SenderEmail = _configuration["EmailSettings:SenderEmail"];

        private readonly string Password = _configuration["EmailSettings:Password"];


        public async Task SendEmailAsync(string to, string subject, string body)
        {

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                SenderName,
                SenderEmail));

            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                SmtpServer,
                Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                SenderEmail,
                Password);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

        }
    }
}

