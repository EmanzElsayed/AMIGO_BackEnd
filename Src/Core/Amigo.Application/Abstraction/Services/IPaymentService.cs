using Amigo.Domain.DTO.Payment;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IPaymentService
    {
        Task<CreatePaymentResponseDTO> CreateStripePaymentAsync(Order order);
        Task HandlePaymentSucceeded(Event stripeEvent);
        Task HandlePaymentFailed(Event stripeEvent);

    }
}
