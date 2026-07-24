using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Amigo.Application.Services
{
    public class EmailService(IConfiguration _configuration) : IEmailService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task SendEmailAsync(string to, string subject, string body, byte[]? qrImage = null)
        {
            var apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY")
                         ?? _configuration["Brevo:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Brevo API Key is missing.");
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

            var emailPayload = new Dictionary<string, object>
            {
                { "sender", new { email = "info@amigoarabe.tours", name = "Amigo Arabe Tours" } },
                { "to", new[] { new { email = to } } },
                { "subject", subject },
                { "htmlContent", body }
            };

            if (qrImage != null && qrImage.Length > 0)
            {
                var base64Image = Convert.ToBase64String(qrImage);

                var attachments = new[]
                {
                    new
                    {
                        content = base64Image,
                        name = "qr.png",
                        cid = "qrCode"
                    }
                };

                emailPayload.Add("attachment", attachments);
            }

            var content = new StringContent(JsonSerializer.Serialize(emailPayload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to send email via Brevo: {errorResponse}");
            }
        }
    }
}