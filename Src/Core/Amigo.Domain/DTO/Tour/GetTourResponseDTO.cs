using Amigo.Domain.DTO.Cancellation;
using Amigo.Domain.DTO.Images;
using Amigo.Domain.DTO.Price;
using Amigo.Domain.DTO.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Tour
{
    public record GetTourResponseDTO
     (
         Guid Id,
         string GuideLanguage,
         string MeetingPoint,
         string Description,

         List<GetImageUrlResponseDTO>? Images,


         TimeSpan Duration,
         Guid DestinationId,
         string Title,
         string Language,
         string Currency,
         string UserType,


         GetCancellationResponseDTO? Cancellation,
         List<string>? Includes,
         List<string>? NotIncludes,

         List<GetPriceResponseDTO>? Prices,
         List<GetTourScheduleResponseDTO>? Schedule,


         bool? IsPitsAllowed,
         bool? IsWheelchairAvailable

     );
}
