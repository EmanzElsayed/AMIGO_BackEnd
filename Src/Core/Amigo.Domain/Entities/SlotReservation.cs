using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;
[Table($"{nameof(SlotReservation)}", Schema = SchemaConstants.tour_schema)]

public class SlotReservation : BaseEntity<Guid>
    {
        public Guid SlotId { get; set; }
        public AvailableSlots Slot { get; set; } = null!;

        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int Quantity { get; set; } // adult + child

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public ReservationStatus Status { get; set; }
    }

