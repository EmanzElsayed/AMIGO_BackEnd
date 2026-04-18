using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Order
{
    public record OrderItemDetailsDTO
    (
        Guid OrderItemId,
        string TourTitle,
        DateOnly TourDate,
        TimeOnly StartTime,
        int? PeopleCount,
        Guid? BookingId,
        string? BookingNumber
    );
}
