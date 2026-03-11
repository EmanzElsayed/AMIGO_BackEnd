using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class BookingTraveler : BaseEntity<Guid>
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public string FullName { get; set; } = null!;
    public string? Nationality { get; set; }
    public string? Phone { get; set; }

    public Guid? PriceCategoryId { get; set; }
    public PriceCategory? PriceCategory { get; set; }
}

