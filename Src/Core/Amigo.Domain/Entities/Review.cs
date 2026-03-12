

namespace Amigo.Domain.Entities
{
    public class Review:BaseEntity<Guid>
    {
        public Guid TourId { get; set; }
        [Required]
        public Tour Tour { get; set; } = null!;


        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public ApplicationUser User { get; set; } = null!;

        public decimal Rate { get; set; }

        public DateOnly Date { get; set; }

        public string? TravelWith { get; set; }

        //Review Image
        public ICollection<ReviewImage> Images { get; set; } = new List<ReviewImage>();
    }
}
