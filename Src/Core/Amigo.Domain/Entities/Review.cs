using Amigo.Domain.Entities.TranslationEntities;

namespace Amigo.Domain.Entities;

[Table($"{nameof(Review)}", Schema = SchemaConstants.review_schema)]

public class Review:BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public decimal Rate { get; set; }
    public DateOnly Date { get; set; }
    public string? TravelWith { get; set; }
    public int HelpfulCount { get; set; }
    public ICollection<ReviewImage> Images { get; set; } = new List<ReviewImage>();
    public ICollection<ReviewVote> Votes { get; set; } = new List<ReviewVote>();
    public ICollection<ReviewTranslation> Translations { get; set; } = new List<ReviewTranslation>();
}
