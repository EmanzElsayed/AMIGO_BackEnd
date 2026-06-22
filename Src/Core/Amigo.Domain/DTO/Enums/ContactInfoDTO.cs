using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Enums
{
    public record ContactInfoDTO
    (
        string? WebsiteLink,
        string? PhoneNumber,
        string? FacebookLink,
        string? YoutubeLink,
        string? InstaLink,
        string? LinkedInLink,
        string? CreatedAt,
        string? Email,
        string? Location
    );
}
