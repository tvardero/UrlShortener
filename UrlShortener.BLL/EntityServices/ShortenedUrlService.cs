using Microsoft.EntityFrameworkCore;
using UrlShortener.BLL.CustomServices;

namespace UrlShortener.BLL.EntityServices;

public class ShortenedUrlService : EFEntityServiceBase<ShortenedUrl, string>
{
    protected override Func<string, Expression<Func<ShortenedUrl, bool>>> PrimaryKeyComparer => hash => entity => entity.Hash == hash;

    public ShortenedUrlService(AppDbContext ctx, HashGeneratorService hashGenerator) : base(ctx)
    {
        _hashGenerator = hashGenerator;
    }

    /// <summary>
    /// Створює нову сутність та генерує всі його дані. Не зберігає ані в контексті, ані в базі даних. 
    /// </summary>
    /// <param name="destinationUrl">Адреса, яку треба скоротити.</param>
    /// <param name="expirationTime">Час через який скорочення перестає працювати.</param>
    /// <returns>Проміс, що повертає створену сутність.</returns>
    public async Task<ShortenedUrl> GenerateEntityAsync(User user, string destinationUrl, TimeSpan expirationTime)
    {
        var normalizedDestinationUrl = new UriBuilder(destinationUrl).Uri.ToString();

        var existing = await _ctx.ShortenedUrls
            .AsTracking()
            .FirstOrDefaultAsync(su => su.DestinationUrl == normalizedDestinationUrl);

        if (existing != null) return existing;

        var utcNow = DateTime.UtcNow;

        var newEntity = new ShortenedUrl()
        {
            Hash = await _hashGenerator.NextAsync(),
            CreatedAtUtc = utcNow,
            ExpiredAtUtc = utcNow + expirationTime,
            DestinationUrl = normalizedDestinationUrl,
            User = user
        };

        return newEntity;
    }

    /// <summary>
    /// Створює нову сутність та генерує всі його дані. Не зберігає ані в контексті, ані в базі даних. 
    /// </summary>
    /// <param name="destinationUrl">Адреса, яку треба скоротити.</param>
    /// <param name="expirationTime">Час через який скорочення перестає працювати.</param>
    /// <returns>Створена сутність.</returns>
    public ShortenedUrl GenerateEntity(User user, string destinationUrl, TimeSpan expirationTime)
    {
        _ctx.Attach(user);
        return GenerateEntityAsync(user, destinationUrl, expirationTime).Result;
    }

    private readonly HashGeneratorService _hashGenerator;
}