using Amigo.Domain.Abstraction;

namespace Amigo.Domain.Entities;

[Table($"{nameof(Destination)}", Schema = SchemaConstants.auth_schema)]

public class Destination : BaseEntity<Guid>, ITranslatable<DestinationTranslation>
{
    public CountryCode CountryCode { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl {  get; set; }
    public string? ImagePublicId { get; set; }
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();

    public ICollection<DestinationTranslation> Translations { get; set; } = new List<DestinationTranslation>();
}
