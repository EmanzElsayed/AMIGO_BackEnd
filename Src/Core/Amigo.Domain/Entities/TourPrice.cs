using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class TourPrice : BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public Guid PriceCategoryId { get; set; }
    public PriceCategory PriceCategory { get; set; } = null!;

    public decimal Price { get; set; }
    public Currency Currency { get; set; }
}

