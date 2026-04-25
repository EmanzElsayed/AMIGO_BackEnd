using Amigo.Domain.DTO.Payment;
using Microsoft.AspNetCore.Http;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class StripePaymentProvider : IPaymentProvider
    {
        public PaymentProvider Provider => PaymentProvider.Stripe;

        public async Task<CreatePaymentResponseDTO> CreatePaymentAsync(Order order)
        {
            var service = new PaymentIntentService();

            var intent = await service.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = (long)(order.TotalAmount * 100),
                Currency = order.Currency.ToString().ToLower(),
                AutomaticPaymentMethods = new()
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
            {
                { "orderId", order.Id.ToString() }
            }
            });

            return new CreatePaymentResponseDTO(
                PaymentIntentId: intent.Id,
                ClientSecret: intent.ClientSecret,
                RedirectUrl:null,
                RequiresRedirect:false
            );
        }

        public Task<string> CapturePaymentAsync(string providerPaymentId)
            => Task.FromResult(providerPaymentId);

        public Task<bool> VerifyWebhookAsync(HttpRequest req, string body)
            => Task.FromResult(true);
    }
}
