using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Entities;
[Table(nameof(Voucher), Schema = SchemaConstants.booking_schema)]

public class Voucher : BaseEntity<Guid>
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public string VoucherNumber { get; set; } = null!; // AMI-2026-0001

    public DateTime IssuedAt { get; set; }


    public string Token { get; set; } = null!;
    public string QRCodeBase64 { get; set; } = null!;

    public bool IsSentByEmail { get; set; }

    public DateTime? SentAt { get; set; }

    public VoucherStatus Status { get; set; }
}

