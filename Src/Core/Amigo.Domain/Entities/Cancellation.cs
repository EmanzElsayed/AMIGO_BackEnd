namespace Amigo.Domain.Entities;

[Table($"{nameof(Cancellation)}", Schema = SchemaConstants.tour_schema)]


public class Cancellation:BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;
    public CancelationPolicyType CancelationPolicyType { get; set; }
    public TimeSpan CancellationBefore  { get; set; }
    public decimal RefundPercentage { get; set; }
    public ICollection<CancellationTranslation> Translations { get; set; } = new List<CancellationTranslation>();

}
