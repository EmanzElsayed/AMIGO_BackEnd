using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Tour
{
    public class TourReviewRowDto
    {
        public Guid TourId { get; set; }

        public decimal Rate { get; set; }
    }
}
