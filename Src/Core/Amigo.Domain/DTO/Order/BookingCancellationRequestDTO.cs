using System;

namespace Amigo.Domain.DTO.Order
{
    
    public record BookingCancellationRequestDTO(
        DateTime RequestedAt,
        string CancelationPolicyType,
        decimal RefundPercentage,
        decimal RefundAmount,
        string Status
    );
}
