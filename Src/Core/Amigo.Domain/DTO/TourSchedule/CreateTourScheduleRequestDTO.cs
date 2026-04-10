using Amigo.Domain.DTO.AvailableSlots;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.TourSchedule
{
    public record CreateTourScheduleRequestDTO
    (
        string? AvailableDateStatus,
        
        DateOnly StartDate,
        List<CreateAvailableSlotsRequestDTO> availableSlots

        
       
    );
}
