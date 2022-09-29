using Microsoft.Extensions.Caching.Memory;
using UrlShortener.BLL.EntityServices;

namespace UrlShortener.MVC.Controllers;

public class RedirectController : Controller
{
    public RedirectController(ShortenedUrlService service, IMemoryCache cache)
    {
        _service = service;
        _cache = cache;
    }

    // [HttpGet]
    public async Task<IActionResult> RedirectToDestination(string hash)
    {
        if (!_cache.TryGetValue(hash, out ShortenedUrl? shortened))
        {
            shortened = await _service.GetByIdAsync(hash);

            if (shortened != null)
                _cache.Set(hash, shortened, shortened.ExpiredAtUtc);
        }

        return shortened != null ? RedirectPermanent(shortened.DestinationUrl) : View("NotFound");
    }

    private readonly ShortenedUrlService _service;
    private readonly IMemoryCache _cache;
}