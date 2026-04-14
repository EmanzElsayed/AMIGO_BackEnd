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
         Language? GuideLanguage,
         string? MeetingPoint,
         string? Description,

         List<ImageUrlsRequestDTO>? Images,


         TimeSpan? Duration,
         Guid? DestinationId,
         string? Title,
         string? Language,
         string? Currency,
         UserType? UserType,


         UpdateCancellationRequestDTO? Cancellation,
         List<string>? Includes,
         List<string>? NotIncludes,
         List<UpdatePriceRequestDTO>? Prices,
         List<UpdateTourScheduleRequestDTO>? Schedule,


         bool? IsPitsAllowed,
         bool? IsWheelchairAvailable

     );
}
