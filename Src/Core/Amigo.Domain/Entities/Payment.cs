using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities
{
    public class Payment:BaseEntity<Guid>
    {
        public Guid OrderId { get; set; }

        [Required]
        public Order Order { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string? Note { get; set; }

        public Currency Currency { get; set; }
        public DateTime PaidAt { get; set; }       
        public string? TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
