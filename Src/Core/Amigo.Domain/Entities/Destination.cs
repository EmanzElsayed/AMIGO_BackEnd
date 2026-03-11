using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class Destination : BaseEntity<Guid>
{
    public CountryCode CountryCode { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public string ImageUrl { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
    public ICollection<DestinationTranslation> Translations { get; set; } = new List<DestinationTranslation>();
}

