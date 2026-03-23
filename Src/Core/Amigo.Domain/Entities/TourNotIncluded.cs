namespace Amigo.Domain.Entities;

[Table($"{nameof(TourNotIncluded)}", Schema = SchemaConstants.tour_schema)]

public class TourNotIncluded: BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    [Required]
    public Tour Tour { get; set; } = null!;

    [Required]
    public string NotIncluded { get; set; } = null!;

    public Language Language { get; set; }
}
