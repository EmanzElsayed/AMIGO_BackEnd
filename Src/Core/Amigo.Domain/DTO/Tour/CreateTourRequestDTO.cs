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
    public record CreateTourRequestDTO
    (
        List<SupportedLanguage>? GuideLanguage,
        string? MeetingPoint,
        string? Description,

        List<ImageUrlsRequestDTO>? Images, 


        TimeSpan Duration,
        Guid DestinationId,
        string Title,
        string Language,
        string Currency,

        List<UserType> UserType,


        List<CreateCancellationRequestDTO>? Cancellation,

        List<string>? Includes,
        List<string>? NotIncludes,

        List<CreatePriceRequestDTO> Prices,

        bool? IsFullTime,
        List <CreateBlackoutDateRequestDTO>? BlackoutDates,
        List<CreateBlackoutWeekDaysRequestDTO>? BlackoutWeekDays,
        List<CreateAvailableSlotsRequestDTO>? Schedule,


        bool IsPitsAllowed = false, 
        bool IsWheelchairAvailable = false 

    );
    
}
