using Amigo.Domain.DTO.Payment;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Amigo.Application.Abstraction.Services
{
    public interface IPaymentOrchestrator
    {
        Task HandleSuccessAsync(PaymentProvider provider, string payload);
        Task HandleFailureAsync(PaymentProvider provider, string payload);
        Task HandleRefundCompleted(
       JsonElement root);
        Task HandleRecoveredSuccessAsync(
           PaymentProvider provider,
           QueryPaymentResponseDTO queryResult);

        Task HandleRecoveredFailureAsync(PaymentProvider provider,
           QueryPaymentResponseDTO queryResult);




    }
}
