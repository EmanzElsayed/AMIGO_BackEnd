using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Price
{
    public class FlatSummeryPriceDTO
    {
        public Guid TourId { get; init; }

        public decimal? RetailPrice { get; init; }

        public decimal? CostPrice { get; init; }
    }
}
