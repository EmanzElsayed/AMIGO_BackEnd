namespace Amigo.Domain.Entities;

public class InterestedTour
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public Guid InterestedInTourId { get; set; }
    public Tour InterestedInTour { get; set; } = null!;

    public int Index { get; set; }
}

