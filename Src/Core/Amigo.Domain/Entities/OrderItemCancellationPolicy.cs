using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities
{
    public class OrderItemCancellationPolicy:BaseEntity<Guid>
    {
        public Guid OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; } = null!;

        public CancelationPolicyType CancelationPolicyType { get; set; }
        public TimeSpan CancellationBefore { get; set; }
        public decimal RefundPercentage { get; set; }
    }
}
