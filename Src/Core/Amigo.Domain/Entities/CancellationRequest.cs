using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;
[Table($"{nameof(CancellationRequest)}", Schema = SchemaConstants.booking_schema)]

public class CancellationRequest : BaseEntity<Guid>
{
    public Guid BookingId { get; set; }

    public Booking Booking { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public CancelationPolicyType cancelationPolicyType { get; set; }
    public decimal RefundPercentage { get; set; }

    public CancellationRequestStatus Status { get; set; }

    public decimal RefundAmount { get; set; }

    public DateTime RequestedAt { get; set; }

    public string? AdminNotes { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
