using Amigo.Domain.DTO.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Booking
{
    public record BookingQRCodeDTO
    (
         string? BookingNumber,
         string BookingStatus,
         DateTime? ConfirmedAt,
         string CustomerName,
         string CustomerEmail,
         List<TravelerDTO> Travelers,
         OrderItemDTO OrderItem,
         PaymentResponseDTO Payment
    );
}
