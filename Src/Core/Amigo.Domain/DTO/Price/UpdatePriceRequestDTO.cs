using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Price
{
    public record UpdatePriceRequestDTO
    (
        
        decimal? Discount,
        string? Type,
        decimal? Cost,
        UserType? UserType,
         DateOnly? SpecialDate,
        string? ActivityType,
        bool? IsMainActivityType
    );
}
