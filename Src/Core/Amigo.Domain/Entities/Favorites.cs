

using System.ComponentModel.DataAnnotations.Schema;

using Amigo.Domain.Entities.Identity;
using Amigo.SharedKernal.Constants;

namespace Amigo.Domain.Entities;

[Table($"{nameof(Favorites)}", Schema = SchemaConstants.auth_schema)]

public class Favorites: BaseEntityWithoutId
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;
    public string UserId { get; set; } = null!;  
    public ApplicationUser User { get; set; } = null!;
}
