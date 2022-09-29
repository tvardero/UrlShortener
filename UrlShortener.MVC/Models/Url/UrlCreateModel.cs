namespace UrlShortener.MVC.Models;

public class UrlCreateModel
{
    [Required, Url]
    public string DestinationUrl { get; init; } = null!;

    public TimeSpan Expiration { get; init; }
}