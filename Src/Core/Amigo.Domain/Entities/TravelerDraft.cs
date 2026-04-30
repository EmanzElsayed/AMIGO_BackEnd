using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities
{
    public class TravelerDraft:BaseEntity<Guid>
    {
        public Guid? OrderItemId { get; set; }
        public OrderItem? OrderItem { get; set; }

        public Guid? CartItemId {  get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string Nationality { get; set; } = null!;
        
        public string? Type { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? PassportNumber { get; set; }
        public string? PhoneNumber { get; set; }



    }
}
