using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Price
{
    public sealed class TourPriceSummaryDto
    {
        public Guid TourId { get; init; }

        public decimal? MaxRetailPrice { get; init; }

        public decimal? MaxCostPrice { get; init; }

       
    }
}
