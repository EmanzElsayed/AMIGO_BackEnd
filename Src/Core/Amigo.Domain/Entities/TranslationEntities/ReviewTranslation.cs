namespace Amigo.Domain.Entities.TranslationEntities;

[Table($"{nameof(ReviewTranslation)}", Schema = SchemaConstants.tour_schema)]
public class ReviewTranslation:BaseEntity<Guid>
{
    public Guid ReviewId { get; set; }
    
    public Review Review { get; set; } = null!;

    
    public string Comment { get; set; } = null!;

    public Language Language {  get; set; }
}
