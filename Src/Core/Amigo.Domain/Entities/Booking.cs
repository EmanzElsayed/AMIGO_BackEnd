using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities
{
    public class Booking:BaseEntity<Guid>
    {
        public Guid OrderId { get; set; }

        [Required]
        public Order Order { get; set; } = null!;

        public Guid AvailableSlotsId { get; set; }
        [Required]
        public AvailableSlots AvailableSlots { get; set; } = null!;

        public BookingStatus Status { get; set; }

        public DateTime? BookingDate { get; set; }

        public ICollection<PeopleBooking> PeopleBookings { get; set; } = new List<PeopleBooking>(); 
    }
}
