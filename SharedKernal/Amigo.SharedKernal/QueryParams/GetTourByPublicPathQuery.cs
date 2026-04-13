namespace Amigo.SharedKernal.QueryParams;

/// URL ->     /{lang}/{destinationSlug}/{tourSlug}</c>

public class GetTourByPublicPathQuery
{
    public string DestinationSlug { get; set; } = null!;

    public string TourSlug { get; set; } = null!;

    public string? Language { get; set; }

    public string? Currency { get; set; }
}
