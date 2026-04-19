using Amigo.Domain.DTO.AvailableSlots;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Price
{
    public record GetTourScheduleResponseDTO
    (
        Guid Id,
        string AvailableDateStatus,

        DateOnly StartDate,
        List<GetAvaialbleSlotResponseDTO>? availableSlots
    );
}
