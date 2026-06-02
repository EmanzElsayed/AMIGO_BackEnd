using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.DTO.Cancellation
{
    public class TourCancellationRowDto
    {
        public Guid TourId { get; set; }

        public bool IsFree { get; set; }
    }
}
