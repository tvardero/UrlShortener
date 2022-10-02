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

    public async Task<IActionResult> RedirectToDestination(string hash)
    {
        if (!_cache.TryGetValue(hash, out ShortenedUrl? shortened))
        {
            shortened = await _service.GetByIdAsync(hash);

            if (shortened != null)
                _cache.Set(hash, shortened, shortened.ExpiredAtUtc);
        }

        if (shortened == null)
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        return RedirectPermanent(shortened.DestinationUrl);
    }

    private readonly ShortenedUrlService _service;
    private readonly IMemoryCache _cache;
}