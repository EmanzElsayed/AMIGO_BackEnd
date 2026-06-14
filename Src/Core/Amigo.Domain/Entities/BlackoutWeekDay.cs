using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities
{
    public class BlackoutWeekDay:BaseEntity<Guid>
    {
        

        public Guid TourId { get; set; }
        public Tour Tour { get; set; } = null!;

        public DayOfWeek DayOfWeek { get; set; }
    }
}
