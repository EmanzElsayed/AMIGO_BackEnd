using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Payment
{
    public record CreatePaymentResponseDTO
   (
        string PaymentIntentId,
        bool RequiresRedirect,
        string? ClientSecret,
        string? RedirectUrl,
        Guid? paymentId = null
    );
}
