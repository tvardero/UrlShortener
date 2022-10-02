using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using UrlShortener.BLL.EntityServices;
using UrlShortener.DataAccess;

namespace UrlShortener.MVC.Controllers;

public class HomeController : Controller
{
    public HomeController(IMapper mapper, UserManager<User> userManager)
    {
        _mapper = mapper;
        this._userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> Urls([FromQuery] GetAllUrls args, [FromServices] IEFEntityService<ShortenedUrl, string> service)
    {
        var (orderBy, orderByDescending) = args.GetSorting();
        var filters = args.GetFilters();
        var includes = new Func<IQueryable<ShortenedUrl>, IIncludableQueryable<ShortenedUrl, object?>>[] { q => q.Include(e => e.User) };

        var (urls, totalCount) = await service.GetAllWithTrackingAsync(
            includes: includes,
            skip: args.Skip,
            take: args.Take,
            orderBy: orderBy,
            orderByDescending: orderByDescending,
            filters: filters);

        var currentUser = await _userManager.GetUserAsync(User);

        bool currentUserIsAdmin = false;
        if (currentUser != null) currentUserIsAdmin = await _userManager.IsInRoleAsync(currentUser, Constants.AdminRoleName);

        var urlModels = _mapper.Map<IEnumerable<UrlModel>>(urls, o =>
        {
            o.AfterMap((_, result) =>
            {
                foreach (var u in result)
                {
                    u.CanDelete = currentUser != null && (currentUserIsAdmin || currentUser?.UserName == u.Username);
                }
            });
        });

        var paginableModel = new PaginableModel<UrlModel>()
        {
            CurrentSkip = args.Skip,
            CurrentTake = args.Take,
            TotalCount = totalCount,
            Entities = urlModels
        };

        return View(paginableModel);
    }

    [HttpGet, Authorize]
    public IActionResult CreateNewUrl() => View();

    [HttpPost, Authorize]
    public async Task<IActionResult> CreateNewUrl(UrlCreateModel model, [FromServices] ShortenedUrlService service)
    {
        if (model.Expiration < TimeSpan.FromMinutes(5)) ModelState.AddModelError(nameof(UrlCreateModel.Expiration), "Expiration time is too short (minimum 5 minutes)");
        if (model.Expiration > TimeSpan.FromDays(365)) ModelState.AddModelError(nameof(UrlCreateModel.Expiration), "Expiration time is too long (maximum 1 year)");
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        var entity = service.GenerateEntity(user, model.DestinationUrl, model.Expiration);

        await service.CreateAsync(entity);

        return RedirectToAction(nameof(Urls));
    }

    [HttpPost, Authorize(Roles = Constants.AdminRoleName)]
    public async Task<IActionResult> DeleteUrl(string hash, [FromServices] IEntityService<ShortenedUrl, string> service)
    {
        var entity = await service.GetByIdAsync(hash);
        if (entity != null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, Constants.AdminRoleName);
                if (isAdmin || entity.User?.Id == user.Id)
                {
                    await service.RemoveAsync(entity);
                }
            }
        }

        return RedirectToAction(nameof(Urls));
    }

    [HttpGet, Authorize(Roles = Constants.AdminRoleName)]
    public async Task<IActionResult> Users([FromQuery] GetAllUsers args)
    {
        var (orderBy, orderByDescending) = args.GetSorting();
        var filters = args.GetFilters();

        var query = _userManager.Users.AsQueryable();

        foreach (var f in filters) query = query.Where(f);

        var totalCount = await query.CountAsync();

        var users = new List<User>();
        if (totalCount > 0)
        {
            if (orderBy != null) query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            query.Skip(args.Skip).Take(args.Take);

            users = await query.ToListAsync();
        }

        var userModels = _mapper.Map<IEnumerable<UserModel>>(users);

        var paginableModel = new PaginableModel<UserModel>()
        {
            CurrentSkip = args.Skip,
            CurrentTake = args.Take,
            Entities = userModels,
            TotalCount = totalCount
        };

        return View(paginableModel);
    }

    [HttpPost, Authorize(Roles = Constants.AdminRoleName)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null) await _userManager.DeleteAsync(user);

        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    public IActionResult About() => View();

    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
}
