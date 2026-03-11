using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class ReviewImage : BaseEntity<Guid>
{
    public Guid ReviewId { get; set; }
    public Review Review { get; set; } = null!;

    public string ImagePath { get; set; } = null!;
}

