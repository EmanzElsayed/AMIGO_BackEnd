using Amigo.SharedKernal.DTOs.Tour;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.TourSchedule
{
    public record UserTourScheduleDetailDTO
    (
        IReadOnlyList<UserTourPriceTierDto> PriceTiers,
        IReadOnlyList<string?>? ActivityTypes,
        IReadOnlyList<DateOnly>? BlackoutDates,
        IReadOnlyList<DayOfWeek>? BlackoutWeekDayes,
        bool? IsFullTime,
        IReadOnlyList<TimeOnly>? AvailableTimes

    );
}
