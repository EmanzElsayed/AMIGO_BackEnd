using Amigo.Domain.DTO.Payment;
using Microsoft.AspNetCore.Http;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Amigo.Application.Services;

public class PaypalPaymentProvider : IPaymentProvider
{
    public PaymentProvider Provider => PaymentProvider.Paypal;

    private readonly PayPalHttpClient _client;
    private readonly IConfiguration _config;

    public PaypalPaymentProvider(PayPalHttpClient client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }

    public async Task<CreatePaymentResponseDTO> CreatePaymentAsync(Domain.Entities.Order order)
    {
        var request = new OrdersCreateRequest();

        request.Prefer("return=representation");

        request.RequestBody(new OrderRequest
        {
            CheckoutPaymentIntent = "CAPTURE",

            PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new PurchaseUnitRequest
            {
                ReferenceId = order.Id.ToString(),
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = order.Currency.ToString(),
                    Value = order.TotalAmount.ToString("F2")
                }
            }
        }
        });

        var response = await _client.Execute(request);

        var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

        var approveUrl = result.Links
            .First(x => x.Rel == "approve")
            .Href;

        return new CreatePaymentResponseDTO(
            PaymentIntentId: result.Id, // PayPal Order ID
            RequiresRedirect: true,
            ClientSecret: null,
            RedirectUrl: approveUrl
        );
    }

    public async Task<string> CapturePaymentAsync(string orderId)
    {
        var request = new OrdersCaptureRequest(orderId);
        request.RequestBody(new OrderActionRequest());

        var response = await _client.Execute(request);
        var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

        return result.Id;
    }

    public async Task<bool> VerifyWebhookAsync(HttpRequest request, string body)
    {
        var authAlgo = request.Headers["Paypal-Auth-Algo"].ToString();
        var transmissionId = request.Headers["Paypal-Transmission-Id"].ToString();
        var certUrl = request.Headers["Paypal-Cert-Url"].ToString();
        var transmissionSig = request.Headers["Paypal-Transmission-Sig"].ToString();
        var transmissionTime = request.Headers["Paypal-Transmission-Time"].ToString();

        var verifyRequest = new
        {
            auth_algo = authAlgo,
            transmission_id = transmissionId,
            cert_url = certUrl,
            transmission_sig = transmissionSig,
            transmission_time = transmissionTime,
            webhook_id = _config["Paypal:WebhookId"],
            webhook_event = JsonDocument.Parse(body).RootElement
        };

        var client = new HttpClient();

        var token = await GetAccessToken();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(
            $"{_config["Paypal:BaseUrl"]}/v1/notifications/verify-webhook-signature",
            verifyRequest
        );

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        return result.GetProperty("verification_status").GetString() == "SUCCESS";
    }

    private async Task<string> GetAccessToken()
    {
        var client = new HttpClient();

        var auth = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_config["Paypal:ClientId"]}:{_config["Paypal:Secret"]}")
        );

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", auth);

        var response = await client.PostAsync(
            $"{_config["Paypal:BaseUrl"]}/v1/oauth2/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            })
        );

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("access_token").GetString();
    }
}