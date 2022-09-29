using Microsoft.EntityFrameworkCore;

namespace UrlShortener.DataAccess.DbEntities;

public class ShortenedUrl
{
    [ForeignKey(nameof(User))]
    public Guid? UserId { get; init; }
    public User? User { get; init; } = default!;

    [Key, Unicode(false), MaxLength(10)]
    public string Hash { get; init; } = null!;

    [Required, Url, Unicode(false), MaxLength(2048)]
    public string DestinationUrl { get; init; } = null!;

    public DateTime CreatedAtUtc { get; init; }
    public DateTime ExpiredAtUtc { get; set; }
}