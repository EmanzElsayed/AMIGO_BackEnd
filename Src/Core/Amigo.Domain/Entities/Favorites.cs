

namespace Amigo.Domain.Entities
{
    public class Favorites: BaseEntityWithoutId
    {
        public Guid TourId { get; set; } // => PK
        [Required]
        public Tour Tour { get; set; } = null!;

        
        public Guid UserId { get; set; }  //=> PK

        [Required]
        public ApplicationUser User { get; set; } = null!;
    }
}
