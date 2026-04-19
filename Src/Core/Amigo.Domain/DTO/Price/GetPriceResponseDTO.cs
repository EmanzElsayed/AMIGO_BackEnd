using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Price
{
    public record GetPriceResponseDTO
     (
        Guid Id,
        decimal Discount,
        string Type,
        decimal Cost,
        UserType UserType
    );
}
