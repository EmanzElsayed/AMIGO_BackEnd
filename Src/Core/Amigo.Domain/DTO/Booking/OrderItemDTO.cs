using Amigo.Domain.DTO.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Booking
{
    public record OrderItemDTO
    (
         Guid OrderItemId,
        
         Guid? TourId,
         string TourTitle,
        string DestinationName,
        DateOnly TourDate,
        TimeOnly StartTime,
        string? MeetingPoint,
        string CancelationPolicyType,
         TimeSpan CancellationBefore,
         decimal RefundPercentage,
         List<OrderedPricesResponseDTO> Prices
    );
}
