namespace Amigo.Domain.Entities;

public class TourCategory
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}

