using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.TourSchedule
{
    public record CreateTourScheduleRequestDTO
    (
        string? AvailableDateStatus,
        Guid TourId,
        DateOnly StartDate,
        DateOnly EndDate
    );
}
