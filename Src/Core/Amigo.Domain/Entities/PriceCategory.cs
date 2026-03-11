using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class PriceCategory : BaseEntity<Guid>
{
    public string Code { get; set; } = null!;

    public ICollection<TourPrice> TourPrices { get; set; } = new List<TourPrice>();
}

