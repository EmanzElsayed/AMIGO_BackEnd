namespace Amigo.Domain.Entities;

[Table($"{nameof(TourImage)}", Schema = SchemaConstants.tour_schema)]

public class TourImage: BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    [Required]
    public Tour Tour { get; set; } = null!;
    [Required]
    public string ImageUrl { get; set; } = null!;
    [Required]

    public string ImagePublicId { get; set; } = null!;
}
