

namespace Amigo.Domain.Entities
{
    public class TourNotIncluded: BaseEntity<Guid>
    {
        public Guid TourId { get; set; }
        [Required]
        public Tour Tour { get; set; } = null!;

        [Required]
        public string NotIncluded { get; set; } = null!;

        public Language Language { get; set; }
    }
}
