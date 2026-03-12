

namespace Amigo.Domain.Entities
{
    public class Price : BaseEntity<Guid>
    {
        public Guid TourId { get; set; }
        [Required]
        public Tour Tour { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!;

        public decimal Cost { get; set; }
    }
}
