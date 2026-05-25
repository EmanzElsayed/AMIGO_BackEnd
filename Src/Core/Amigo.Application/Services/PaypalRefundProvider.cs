using System;
using System.Collections.Generic;
using System.Text;

using Amigo.Domain.DTO.Refund;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Payments;
using PayPalHttp;
using System.Text.Json;
namespace Amigo.Application.Services;

public class PaypalRefundProvider : IRefundProvider
{
    private readonly PayPalHttpClient _client;

    public PaymentProvider Provider
        => PaymentProvider.Paypal;

    public PaypalRefundProvider(
        PayPalHttpClient client)
    {
        _client = client;
    }

    public async Task<RefundResultDTO> RefundAsync(
        string captureId,
        decimal amount,
        CurrencyCode currency,
        string idempotencyKey)
    {
        try
        {
            var request =
                new CapturesRefundRequest(captureId);

            request.Headers.Add(
                "PayPal-Request-Id",
                idempotencyKey);

            request.RequestBody(new RefundRequest
            {
                Amount = new Money
                {
                    CurrencyCode = currency.ToString(),
                    Value = amount.ToString("F2")
                }
            });

            var response =
                await _client.Execute(request);

            var result =
                response.Result<PayPalCheckoutSdk.Payments.Refund>();

            return new RefundResultDTO
            {
                Success =
                    result.Status == "COMPLETED",

                RefundId = result.Id,

                RawResponse =
                System.Text.Json.JsonSerializer.Serialize(
                new
                {
                    result.Id,
                    result.Status,
                    Amount = result.Amount?.Value,
                    Currency = result.Amount?.CurrencyCode,
                    result.CreateTime
                })
            };
        }
        catch (Exception ex)
        {
            return new RefundResultDTO
            {
                Success = false,
                FailureReason = ex.Message
            };
        }
    }
}
