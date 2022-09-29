using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using UrlShortener.BLL.EntityServices;

namespace UrlShortener.MVC.Controllers.ApiControllers;

[Authorize]
public class ShortenedUrlApiController : EFApiControllerBase<ShortenedUrl, string, GetAllUrls, UrlModel, UrlCreateModel, UrlUpdateModel>
{
    protected override Func<ShortenedUrl, string> PrimaryKeySelector => e => e.Hash;
    protected override Func<IQueryable<ShortenedUrl>, IIncludableQueryable<ShortenedUrl, object?>>[]? Includes =>
        new Func<IQueryable<ShortenedUrl>, IIncludableQueryable<ShortenedUrl, object?>>[]
        {
            q => q.Include(e => e.User)
        };

    public ShortenedUrlApiController(ShortenedUrlService service, IMapper mapper, UserManager<User> userManager) : base(service, mapper)
    {
        _service = service;
        _userManager = userManager;
    }

    [AllowAnonymous]
    public override Task<IActionResult> GetAll([FromQuery] GetAllUrls args) => base.GetAll(args);

    [AllowAnonymous]
    public override Task<IActionResult> GetById(string id) => base.GetById(id);

    public override async Task<IActionResult> Create(UrlCreateModel dto)
    {
        var user = await _userManager.GetUserAsync(User);
        var entity = await _service.GenerateEntityAsync(user, dto.DestinationUrl, dto.Expiration);

        var result = await _service.CreateAsync(entity);

        return CreatedAtAction(nameof(GetById), new { id = PrimaryKeySelector(result) }, _mapper.Map<UrlModel>(result));
    }

    protected readonly new ShortenedUrlService _service;
    private readonly UserManager<User> _userManager;
}