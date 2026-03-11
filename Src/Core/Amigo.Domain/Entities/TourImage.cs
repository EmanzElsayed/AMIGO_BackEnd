using Amigo.Domain.Enum;
using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class TourMedia : BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;

    public MediaType MediaType { get; set; }
    public string Url { get; set; } = null!;
    public bool IsCover { get; set; }
    public int SortOrder { get; set; }
}

