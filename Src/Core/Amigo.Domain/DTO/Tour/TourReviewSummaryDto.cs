using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Tour
{
    public sealed class TourReviewSummaryDto
    {
        public Guid TourId { get; init; }

        public int ReviewCount { get; init; }

        public decimal? AverageRating { get; init; }
    }
}
