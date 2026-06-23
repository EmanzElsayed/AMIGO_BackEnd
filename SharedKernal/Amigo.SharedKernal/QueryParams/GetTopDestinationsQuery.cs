namespace Amigo.SharedKernal.QueryParams;

public class GetTopDestinationsQuery
{
        
    public string? CountryCode { get; set; }
    private const int DefaultPageSize = 5;
    private const int MaxPageSize = 50;

    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    private int pageSize = DefaultPageSize;

    public int PageSize
    {
        get { return pageSize; }
        set { pageSize = value > MaxPageSize ? MaxPageSize : (value <= 0 ? DefaultPageSize : value); }
    }
}
