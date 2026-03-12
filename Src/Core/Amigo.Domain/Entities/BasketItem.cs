using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities
{
    public class BasketItem:BaseEntity<Guid>
    {
        public Guid BasketId { get; set; }
        [Required]
        public Basket Basket { get; set; } = null!;

        public Guid AvailableSlotsId { get; set; }
        [Required]
        public AvailableSlots AvailableSlots { get; set; } = null!;

        public decimal Price { get; set; }
        public int Quantity {  get; set; }
    }
}
