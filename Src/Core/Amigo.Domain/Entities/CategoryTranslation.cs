using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class CategoryTranslation : BaseEntity<Guid>
{
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Language Language { get; set; }
    public string Name { get; set; } = null!;
}

