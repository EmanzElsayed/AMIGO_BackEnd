using Amigo.SharedKernal.Entities;

namespace Amigo.Domain.Entities;

public class Cart : BaseEntity<Guid>
{
    public Guid UserId { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}

