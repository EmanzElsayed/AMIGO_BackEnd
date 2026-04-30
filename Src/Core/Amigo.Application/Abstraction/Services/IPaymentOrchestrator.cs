using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IPaymentOrchestrator
    {
        Task HandleSuccessAsync(PaymentProvider provider, string payload);
        Task HandleFailureAsync(PaymentProvider provider, string payload);
    }
}
