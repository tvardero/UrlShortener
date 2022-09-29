namespace UrlShortener.MVC.Models;

public class UrlModel
{
    public string Hash { get; init; } = null!;
    public string DestinationUrl { get; init; } = null!;
    public string? Username { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime ExpiredAtUtc { get; init; }
}