using Amigo.Domain.DTO.Payment;
using Microsoft.AspNetCore.Http;
using Stripe.Events;
using System.Net.Http.Json;
using System.Text.Json;

namespace Amigo.Application.Services
{
    public class PayTabsPaymentProvider : IPaymentProvider
    {
        public PaymentProvider Provider => PaymentProvider.PayTabs;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        public PayTabsPaymentProvider(HttpClient client, IConfiguration config, IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
        {
            _httpClient = client;
            _config = config;
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;

        }

        public Task<CapturePaymentResponseDTO> CapturePaymentAsync(string providerPaymentId)
        {
            throw new NotImplementedException();
        }

        public async Task<CreatePaymentResponseDTO> CreatePaymentAsync(Order order, string requestId, string? paymentToken)
        {
            if (string.IsNullOrWhiteSpace(paymentToken))
                throw new Exception("Payment token missing");

            var request = new
            {
                profile_id = _config["PayTabs:ProfileId"],

                tran_type = "sale",

                tran_class = "ecom",

                cart_id = order.Id.ToString(),

                cart_currency = order.Currency.ToString(),

                cart_amount = order.TotalAmount,

                payment_token = paymentToken,

                callback = $"{_config["BackEnd:url"]}/api/webhook/paytabs", // webhook url

                @return = _config["PayTabs:ReturnUrl"]
            };
            var message =
            new HttpRequestMessage(
                HttpMethod.Post,
                "payment/request");

            message.Headers.Add(
                "authorization",
                _config["PayTabs:ServerKey"]);

            message.Content =
                JsonContent.Create(request);

            var response =
                await _httpClient.SendAsync(message);
            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content
                        .ReadAsStringAsync();

                throw new Exception(error);
            }
            var result =
                await response.Content
                    .ReadFromJsonAsync<PayTabsResponseDTO>();

            return new CreatePaymentResponseDTO(
                PaymentIntentId: result.TranRef,
                RequiresRedirect: !string.IsNullOrEmpty(
                    result.RedirectUrl),
                ClientSecret: null,
                RedirectUrl: result.RedirectUrl
            );
        }
        public async Task<QueryPaymentResponseDTO>
        QueryTransactionAsync(string tranRef)
        {
            var request = new
            {
                profile_id = _config["PayTabs:ProfileId"],
                tran_ref = tranRef
            };

            var message = new HttpRequestMessage(
                HttpMethod.Post,
                "payment/query");

            message.Headers.Add(
                "authorization",
                _config["PayTabs:ServerKey"]);

            message.Content =
                JsonContent.Create(request);

            var response =
                await _httpClient.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync();

                throw new Exception(error);
            }

            var json =
                await response.Content.ReadAsStringAsync();

            var result =
                JsonSerializer.Deserialize<
                    PayTabsQueryResponseDto>(json);

            return new QueryPaymentResponseDTO(
               
                Status:
                    result?.PaymentResult?.ResponseStatus,
                ProviderReferenceId:
                    result?.TranRef ?? tranRef,
                RawResponse: json
            );
        }
        public Task<bool> VerifyWebhookAsync(HttpRequest request, string body)
        {
            throw new NotImplementedException();
        }
    }
}
