using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Order
{
    public record OrderDetailsForAdminResponseDTO
    (
        Guid OrderId,
        string UserName,
        string UserEmail,
        string Status,
        decimal TotalAmount,
        string Currency,
        DateTime? OrderDate,
        DateTime ExpiresAt,
        List<PaymentResponseDTO> Payments,
        List<OrderItemDetailsDTO> Items

    );
}
