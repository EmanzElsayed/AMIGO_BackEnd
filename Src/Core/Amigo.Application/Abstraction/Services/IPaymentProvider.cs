using Amigo.Domain.DTO.Payment;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IPaymentProvider
    {
        PaymentProvider Provider { get; }

        Task<CreatePaymentResponseDTO> CreatePaymentAsync(Order order, string requestId,string? paymentToken);
        Task<CapturePaymentResponseDTO> CapturePaymentAsync(string providerPaymentId);


        Task<QueryPaymentResponseDTO> QueryTransactionAsync(
       string providerReferenceId);
        Task<bool> VerifyWebhookAsync(HttpRequest request, string body);

    }
}
