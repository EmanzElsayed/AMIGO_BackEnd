using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Price
{
    public record CreatePriceRequestDTO
    (
        decimal? Discount,
        string Type,
        decimal Cost,
        UserType UserType
    );
}
