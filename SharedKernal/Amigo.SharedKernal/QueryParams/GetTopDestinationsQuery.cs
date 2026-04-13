namespace Amigo.SharedKernal.QueryParams;

public class GetTopDestinationsQuery
{
    public int Take { get; set; } = 10;

    public string? Language { get; set; }

    public string? Currency { get; set; }
}
