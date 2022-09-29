namespace UrlShortener.MVC.Models;

public class GetAllUrls : IGetAllModel<ShortenedUrl>
{
    public enum OrderByEnum : byte
    {
        NotSpecified = 0,
        Hash = NotSpecified,
        Username,
        Destination,
        CreatedAt,
        ExpiredAt
    }

    // Пагінація
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 20;

    // Сортування
    [EnumDataType(typeof(OrderByEnum))]
    public OrderByEnum OrderBy { get; init; } = OrderByEnum.NotSpecified;
    public bool OrderByDescending { get; init; } = false;

    // Фільтри
    public string? Username { get; init; }
    public string? Destination { get; init; }
    public DateTime? CreatedAtUtcMin { get; init; }
    public DateTime? CreatedAtUtcMax { get; init; }
    public DateTime? ExpiredAtUtcMin { get; init; }
    public DateTime? ExpiredAtUtcMax { get; init; }

    public Expression<Func<ShortenedUrl, bool>>[] GetFilters()
    {
        return new Expression<Func<ShortenedUrl, bool>>[]
        {
            e => string.IsNullOrWhiteSpace(Username) || (e.User != null && e.User.UserName.Contains(Username.Trim())),
            e => string.IsNullOrWhiteSpace(Destination) || e.DestinationUrl.Contains(Destination.Trim()),
            e => CreatedAtUtcMin == null || e.CreatedAtUtc >= CreatedAtUtcMin,
            e => CreatedAtUtcMax == null || e.CreatedAtUtc <= CreatedAtUtcMax,
            e => ExpiredAtUtcMin == null || e.ExpiredAtUtc >= ExpiredAtUtcMin,
            e => ExpiredAtUtcMax == null || e.ExpiredAtUtc <= ExpiredAtUtcMax
        };
    }

    public (Expression<Func<ShortenedUrl, object?>>? OrderBy, bool OrderByDescending) GetSorting()
    {
        Expression<Func<ShortenedUrl, object?>>? orderBy = OrderBy switch
        {
            OrderByEnum.Hash => e => e.Hash,
            OrderByEnum.Username => e => e.User!.UserName,
            OrderByEnum.Destination => e => e.DestinationUrl,
            OrderByEnum.CreatedAt => e => e.CreatedAtUtc,
            OrderByEnum.ExpiredAt => e => e.ExpiredAtUtc,
            _ => null,
        };

        return (orderBy, OrderByDescending);
    }
}