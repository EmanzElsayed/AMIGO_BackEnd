namespace Amigo.Domain.Entities.TranslationEntities;

[Table($"{nameof(CancellationTranslation)}", Schema = SchemaConstants.tour_schema)]
public class CancellationTranslation:BaseEntity<Guid>
{
    public Guid CancellationId { get; set; }
   
    public Cancellation Cancellation { get; set; } = null!;

   
    public string Description { get; set; } = null!;

    public Language Language { get; set; }
}
