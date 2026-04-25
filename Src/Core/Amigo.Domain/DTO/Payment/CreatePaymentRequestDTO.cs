using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Payment
{
    public record CreatePaymentRequestDTO(
         Guid OrderId,
         PaymentProvider Provider
    );
}
