using Amigo.Domain.DTO.Cancellation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Order
{
    public record OrderItemDetailsDTO
    (
        Guid OrderItemId,
        Guid? BookingId,
        string? BookingNumber,
        string? BookingStatus,
        Guid? TourId,
        
        string TourTitle,
        string TourSlug,
        string DestinationName,
        DateOnly TourDate,
        TimeOnly StartTime,
        string? MeetingPoint,
        List<GetCancellationResponseDTO>? CancellationPloicy,
         List<OrderedPricesResponseDTO> Prices,
        string? TourImage = null

    );
}
