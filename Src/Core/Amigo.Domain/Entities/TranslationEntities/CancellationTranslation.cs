

namespace Amigo.Domain.Entities.TranslationEntities
{
    public class CancellationTranslation:BaseEntity<Guid>
    {
        public Guid CancellationId { get; set; }
        [Required]
        public Cancellation Cancellation { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public Language Language { get; set; }
    }
}
