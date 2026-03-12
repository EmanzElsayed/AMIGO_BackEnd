using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities
{
    public class Basket:BaseEntity<Guid>
    {

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public ApplicationUser User { get; set; } = null!;
        //Total Amount => calculated
        public decimal TotalAmount { get; set; }

        public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();
    }
}
