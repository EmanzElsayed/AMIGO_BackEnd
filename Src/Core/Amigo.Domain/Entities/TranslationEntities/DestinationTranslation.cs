

namespace Amigo.Domain.Entities.TranslationEntities
{
    public class DestinationTranslation:BaseEntity<Guid>
    {
        public Guid DestinationId { get; set; }
        [Required]
        public Destination Destination { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public Language Language { get; set; }
    }
}
