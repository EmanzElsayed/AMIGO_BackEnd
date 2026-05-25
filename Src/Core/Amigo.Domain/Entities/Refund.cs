using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;
[Table($"{nameof(Refund)}", Schema = SchemaConstants.booking_schema)]

public class Refund : BaseEntity<Guid>
{
    public Guid BookingId { get; set; }

    [Required]
    public Booking Booking { get; set; } = null!;

    public Guid PaymentId { get; set; }
    public Payment Payment { get; set; } 
    public Guid CancellationRequestId { get; set; }
    public CancellationRequest CancellationRequest { get; set; }
    public decimal Amount { get; set; }

    public RefundStatus Status { get; set; }

    public string? ProviderRefundId { get; set; }

    public DateTime RequestedAt { get; set; }
    public string? ProviderResponseJson { get; set; }
    public string? FailureReason { get; set; }
    public string? IdempotencyKey { get; set; }
    public DateTime? RefundedAt { get; set; }
}
