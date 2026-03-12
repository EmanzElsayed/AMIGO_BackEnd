

namespace Amigo.Domain.Entities
{
    public class Favorites: BaseEntityWithoutId
    {
        public Guid TourId { get; set; } // => PK
        [Required]
        public Tour Tour { get; set; } = null!;


        [Required]
        public string UserId { get; set; } = null!;  //=> PK

        [Required]
        public ApplicationUser User { get; set; } = null!;
    }
}
