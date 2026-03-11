using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class Category : BaseEntity<Guid>
{
    public string Icon { get; set; } = null!;

    public ICollection<CategoryTranslation> Translations { get; set; } = new List<CategoryTranslation>();
    public ICollection<TourCategory> Tours { get; set; } = new List<TourCategory>();
}

