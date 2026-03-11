using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class BookingItem : BaseEntity<Guid>
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public Guid PriceCategoryId { get; set; }
    public PriceCategory PriceCategory { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

