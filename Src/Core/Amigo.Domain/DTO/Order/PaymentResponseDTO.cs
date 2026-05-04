using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Order
{
    public record PaymentResponseDTO
    (
         Guid PaymentId,
         decimal PaidAmount,
          string? PaymentMethod,
          string PaymentStatus,
          string PaidCurrency,
          DateTime PaidAt,
          string? Provider,
          string? FailureReason
    );
}
