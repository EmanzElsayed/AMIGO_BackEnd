namespace Amigo.SharedKernal.QueryParams;

public class GetTopDestinationsQuery
{
    public string? Language { get; set; }
    public string? Currency { get; set; }

    private const int DefaultPageSize = 5;
    private const int MaxPageSize = 50;

    public int PageNumber { get; set; } = 1;

    private int pageSize = DefaultPageSize;

    public int PageSize
    {
        get { return pageSize; }
        set { pageSize = value > MaxPageSize ? MaxPageSize : (value <= 0 ? DefaultPageSize : value); }
    }
}
