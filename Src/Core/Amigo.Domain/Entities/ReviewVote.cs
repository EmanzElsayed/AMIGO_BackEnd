using System.ComponentModel.DataAnnotations.Schema;
using Amigo.SharedKernal.Entities;
using Amigo.SharedKernal.Constants;

namespace Amigo.Domain.Entities;

[Table($"{nameof(ReviewVote)}", Schema = SchemaConstants.review_schema)]
public class ReviewVote : BaseEntity<Guid>
{
 
    public Guid ReviewId { get; set; }
    public Review Review { get; set; } = null!;
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public string? IpAddress { get; set; }
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
}
