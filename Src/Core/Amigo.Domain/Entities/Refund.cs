using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;
[Table($"{nameof(Refund)}", Schema = SchemaConstants.booking_schema)]

public class Refund : BaseEntity<Guid>
{
    public Guid PaymentId { get; set; }

    public Payment Payment { get; set; } = null!;

    public decimal Amount { get; set; }

    public RefundStatus Status { get; set; }

    public string? ProviderRefundId { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? RefundedAt { get; set; }
}
