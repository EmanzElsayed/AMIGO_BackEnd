using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.User
{
   public record UserInfoResponseDTO
   (
        string FullName,
        string Email,
        string? Phone,
        string? ImageUrl,
        string? Gender,
        DateOnly? BirthDate,
        string? Nationality,
        string? Language,
        string? BuildingNumber,
        string? City,
        string? Country,
        string UserType
   );
}
