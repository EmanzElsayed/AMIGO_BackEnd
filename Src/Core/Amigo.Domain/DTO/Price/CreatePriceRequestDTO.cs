using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Price
{
    public record CreatePriceRequestDTO
    (
        decimal? Discount,
        Guid TourId,
        string Type,
        decimal Cost
    );
}
