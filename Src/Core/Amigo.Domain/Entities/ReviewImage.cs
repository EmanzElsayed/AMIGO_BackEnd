

namespace Amigo.Domain.Entities
{
    public class ReviewImage:BaseEntity<Guid>
    {
        public Guid ReviewId { get; set; }
        [Required]
        public Review Review { get; set; } = null!;
        [Required]
        public string Image { get; set; } = null!;
    }
}
