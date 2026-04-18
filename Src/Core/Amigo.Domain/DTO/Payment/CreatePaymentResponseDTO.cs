using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Payment
{
    public record CreatePaymentResponseDTO
    (
        string PaymentIntentId,
        string ClientSecret
        
    );
}
