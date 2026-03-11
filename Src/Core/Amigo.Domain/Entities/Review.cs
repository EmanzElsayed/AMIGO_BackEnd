using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class Review : BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public Guid UserId { get; set; }
    public int Rate { get; set; }
    public DateTime Date { get; set; }
    public string? TravelWith { get; set; }

    public ICollection<ReviewImage> Images { get; set; } = new List<ReviewImage>();
}

