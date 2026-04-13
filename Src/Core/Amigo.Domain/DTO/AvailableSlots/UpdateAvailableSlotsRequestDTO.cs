using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.AvailableSlots
{
    public record UpdateAvailableSlotsRequestDTO
    (
        Guid? Id,
        TimeOnly? StartTime,
        int? MaxCapacity,
        string? AvailableTimeStatus
    );
}
