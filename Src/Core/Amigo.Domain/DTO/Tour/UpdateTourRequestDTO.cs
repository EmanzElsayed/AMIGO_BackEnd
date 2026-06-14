using Amigo.Domain.DTO.AvailableSlots;
using Amigo.Domain.DTO.BlackoutDate;
using Amigo.Domain.DTO.BlackoutWeekDays;
using Amigo.Domain.DTO.Cancellation;
using Amigo.Domain.DTO.Images;
using Amigo.Domain.DTO.Price;
using Amigo.Domain.DTO.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Tour
{
    public record UpdateTourRequestDTO
     (
         List<SupportedLanguage>? GuideLanguage,
         string? MeetingPoint,
         string? Description,

         List<ImageUrlsRequestDTO>? Images,


         TimeSpan? Duration,
         Guid? DestinationId,
         string? Title,
         string? Language,
         string? Currency,
         List<UserType>? UserType,


         List<UpdateCancellationRequestDTO>? Cancellation,
         List<string>? Includes,
         List<string>? NotIncludes,

         List<UpdatePriceRequestDTO>? Prices,

         bool? IsFullTime,

        List<CreateBlackoutDateRequestDTO>? BlackoutDates,
        List<CreateBlackoutWeekDaysRequestDTO>? BlackoutWeekDays,

        List<UpdateAvailableSlotsRequestDTO>? Schedule,



         bool? IsPitsAllowed,
         bool? IsWheelchairAvailable

     );
}
