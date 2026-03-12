using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities
{
    public class OrderItem:BaseEntity<Guid>
    {
        public Guid OrderId { get; set; }

        [Required]
        public Order Order { get; set; } = null!;
        
        public Guid AvailableSlotsId { get; set; }
        [Required]
        public AvailableSlots AvailableSlots { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

    }
}
