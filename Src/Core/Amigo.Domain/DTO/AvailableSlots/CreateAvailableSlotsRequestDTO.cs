using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.AvailableSlots
{
    public record CreateAvailableSlotsRequestDTO
    (
        Guid TourScheduleId,
        TimeOnly StartTime,
        TimeOnly EndTime,
        int MaxCapacity,
        string? AvailableTimeStatus

     );
}
