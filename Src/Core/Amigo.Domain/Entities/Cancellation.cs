

namespace Amigo.Domain.Entities
{
    public class Cancellation:BaseEntity<Guid>
    {
        public Guid TourId { get; set; }
        [Required]
        public Tour Tour { get; set; } = null!;

        public CancelationPolicyType CancelationPolicyType { get; set; }

        public TimeOnly HoursBefore  { get; set; }
        public decimal RefundPercentage { get; set; }

    }
}
