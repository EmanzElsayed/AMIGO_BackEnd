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
        Language? GuideLanguage,
        string? MeetingPoint,
        string? Description,

        List<ImageUrlsRequestDTO>? Images, 


        TimeSpan Duration,
        Guid DestinationId,
        string Title,
        string Language,
        string Currency,

        CreateCancellationRequestDTO Cancellation,

        List<string>? Includes,
        List<string>? NotIncludes,
        UserType UserType,
        List<CreatePriceRequestDTO> Prices,
        List<CreateTourScheduleRequestDTO> Schedule,

        bool IsPitsAllowed = false, 
        bool IsWheelchairAvailable = false 

    );
    
}
