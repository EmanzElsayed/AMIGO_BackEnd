using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class Payment : BaseEntity<Guid>
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public DateTime PaidAt { get; set; }

    public string Provider { get; set; } = null!;
    public string ProviderReference { get; set; } = null!;
}

