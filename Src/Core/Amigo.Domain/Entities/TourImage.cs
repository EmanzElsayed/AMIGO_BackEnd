namespace Amigo.Domain.Entities;

[Table($"{nameof(TourImage)}", Schema = SchemaConstants.tour_schema)]

public class TourImage: BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    [Required]
    public Tour Tour { get; set; } = null!;
    [Required]
    public string Image { get; set; } = null!;

}
