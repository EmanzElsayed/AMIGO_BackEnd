using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Order
{
    public record OrderDetailsDTO
    (
        Guid OrderId,
        string Status,
        decimal TotalAmount,
        string Currency,
        DateTime? OrderDate,
        DateTime ExpiresAt,
        List<PaymentResponseDTO> Payments,
        List<OrderItemDetailsDTO> Items
    );
}
