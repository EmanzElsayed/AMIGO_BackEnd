

namespace Amigo.Domain.Entities
{
    public class Review:BaseEntity<Guid>
    {
        public Guid TourId { get; set; }
        [Required]
        public Tour Tour { get; set; } = null!;

        
        public Guid UserId { get; set; } 

        [Required]
        public ApplicationUser User { get; set; } = null!;

        public decimal Rate { get; set; }

        public DateOnly Date { get; set; }

        public string? TravelWith { get; set; }

        //Review Image
        public ICollection<ReviewImage> Images { get; set; } = new List<ReviewImage>();
    }
}
