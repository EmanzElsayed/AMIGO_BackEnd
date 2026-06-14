using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.BlackoutWeekDays
{
    public record GetBlackoutWeekDaysResponseDTO
    (
      Guid Id,
      DayOfWeek DayOfWeek

    );
}
