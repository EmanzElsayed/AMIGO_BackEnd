using Amigo.Domain.DTO.AvailableSlots;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.TourSchedule
{
    public record UpdateTourScheduleRequestDTO
    (
        Guid? Id,
        string? AvailableDateStatus,

        DateOnly? StartDate,
        List<UpdateAvailableSlotsRequestDTO>? availableSlots
    );
}
