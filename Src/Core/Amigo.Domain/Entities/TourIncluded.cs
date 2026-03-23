namespace Amigo.Domain.Entities;

[Table($"{nameof(TourIncluded)}", Schema = SchemaConstants.tour_schema)]

public class TourIncluded: BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    [Required]
    public Tour Tour { get; set; } = null!;
    
    [Required]
    public string Included { get; set; } = null!;

    public Language Language { get; set; }
    

}
