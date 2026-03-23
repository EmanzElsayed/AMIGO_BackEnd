namespace Amigo.Domain.Entities;

[Table($"{nameof(ReviewImage)}", Schema = SchemaConstants.review_schema)]

public class ReviewImage:BaseEntity<Guid>
{
    public Guid ReviewId { get; set; }
    
    public Review Review { get; set; } = null!;
   
    public string Image { get; set; } = null!;
}
