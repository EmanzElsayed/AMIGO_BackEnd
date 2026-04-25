using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.AvailableSlots
{
    public record GetAvaialbleSlotResponseDTO
    (
        
        Guid Id,
        TimeOnly StartTime,
        string AvailableTimeStatus,
        int MaxCapacity,

      int ReservedCount
    );    
    
}
