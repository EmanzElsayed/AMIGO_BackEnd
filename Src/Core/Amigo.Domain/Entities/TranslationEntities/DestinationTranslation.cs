namespace Amigo.Domain.Entities.TranslationEntities;

[Table($"{nameof(DestinationTranslation)}", Schema = SchemaConstants.tour_schema)]
public class DestinationTranslation:BaseEntity<Guid>
{
    public Guid DestinationId { get; set; }
  
    public Destination Destination { get; set; } = null!;

  
    public string Name { get; set; } = null!;

    public Language Language { get; set; }
}
