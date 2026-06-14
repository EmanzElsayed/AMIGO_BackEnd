namespace Amigo.Domain.Entities;

[Table($"{nameof(Price)}", Schema = SchemaConstants.tour_schema)]

public class Price : BaseEntity<Guid>
{
    public Guid TourId { get; set; }
    public Tour Tour { get; set; } = null!;


    //special date 
    public DateOnly? SpecialDate { get; set; }

    //main activity type if exist
    public bool? IsMainActivityType { get; set; } 

    public decimal Cost { get; set; }

    public UserType UserType { get; set; } //vIP or Public 

    public decimal Discount { get; set; } = 0;
    public decimal RetailPrice => Cost * (1 - Discount  / 100m);

    public ICollection<PriceTranslation> Translations { get; set; } = new List<PriceTranslation>();
    

}
