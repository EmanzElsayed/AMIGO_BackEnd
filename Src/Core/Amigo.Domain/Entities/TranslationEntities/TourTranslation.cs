namespace Amigo.Domain.Entities.TranslationEntities;

[Table($"{nameof(TourTranslation)}", Schema = SchemaConstants.tour_schema)]

public class TourTranslation:BaseEntity<Guid>
{
    public Guid TourId { get; set; }
   
    public Tour Tour { get; set; } = null!;

   
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public Language Language { get; set; }

}
