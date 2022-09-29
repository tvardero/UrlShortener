using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortener.BLL.EntityServices;
using UrlShortener.DataAccess;

namespace UrlShortener.MVC.Controllers;

public class HomeController : Controller
{
    public HomeController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> Urls([FromQuery] GetAllUrls args, [FromServices] IEntityService<ShortenedUrl, string> service)
    {
        var (orderBy, orderByDescending) = args.GetSorting();
        var filters = args.GetFilters();

        var (urls, totalCount) = await service.GetAllAsync(args.Skip, args.Take, orderBy, orderByDescending, filters);

        var urlModels = _mapper.Map<IEnumerable<UrlModel>>(urls);

        var paginableModel = new PaginableModel<UrlModel>()
        {
            CurrentSkip = args.Skip,
            CurrentTake = args.Take,
            TotalCount = totalCount,
            Entities = urlModels
        };

        return View(paginableModel);
    }

    [HttpGet, Authorize(Roles = Constants.AdminRoleName)]
    public async Task<IActionResult> Users([FromQuery] GetAllUsers args, [FromServices] UserManager<User> userManager)
    {
        var (orderBy, orderByDescending) = args.GetSorting();
        var filters = args.GetFilters();

        var query = userManager.Users.AsQueryable();

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

    [HttpGet]
    public IActionResult About() => View();

    private readonly IMapper _mapper;
}
