using Amigo.Domain.DTO.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Tour
{
    public record CreateTourRequestDTO
    (
        decimal? Discount, // we should make discount for vip and public
        string? GuideLanguage,
        string? MeetingPoint,
        string? Description,

        List<ImageUrlsRequestDTO>? Images, 


        TimeSpan Duration,
        Guid DestinationId,
        string Title,
        string Language,
        string Currency,

        bool IsPitsAllowed = false, // ask for default
        bool IsWheelchairAvailable = false, // ask for default
        bool IsVip = false,
        bool IsPublic = false

    );
    
}
