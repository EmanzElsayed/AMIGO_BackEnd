using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Order
{
    public record PaymentResponseDTO
    (
        Guid PaymentId,
         decimal TotalAmount,
         PaymentMethod? PaymentMethod,
          PaymentStatus Status,
          CurrencyCode Currency,
          DateTime PaidAt,
          PaymentProvider Provider,
          string? RawResponseJson,
          string? FailureReason
    );
}
