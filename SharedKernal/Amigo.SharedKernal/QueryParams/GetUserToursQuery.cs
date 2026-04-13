namespace Amigo.SharedKernal.QueryParams;

public class GetUserToursQuery
{
    public Guid DestinationId { get; set; }

    public string? Language { get; set; }

    public string? Currency { get; set; }

    public string? CountryCode { get; set; }

    public string? DestinationName { get; set; }

    public string? TourTitle { get; set; }

    public string? Category { get; set; }

    public string? GuideLanguage { get; set; }

    public double? MinDurationHours { get; set; }

    public double? MaxDurationHours { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? AvailabilityDate { get; set; }

    public bool? IsPitsAllowed { get; set; }

    public bool? IsWheelchairAvailable { get; set; }

    public string? UserType { get; set; }

    public bool? FreeCancellation { get; set; }

    public bool? OnlyInUserLanguage { get; set; }

    public bool? HotelPickup { get; set; }

    /// <summary>When true, only tours that have at least one available (non-sold-out) slot.</summary>
    public bool? RequireAvailableSlots { get; set; }

    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    private int _pageSize = 12;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > 48 ? 12 : value;
    }
}
