using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using UrlShortener.BLL.EntityServices;

namespace UrlShortener.MVC.Controllers;

/// <summary>
/// RESTlike абстрактний контроллер. 
/// </summary>
/// <typeparam name="TEntity">Тип ентіті з EF контексту.</typeparam>
/// <typeparam name="TKey">Тип первинного ключа ентіті.</typeparam>
/// <typeparam name="TGetAllArguments">Об'єкт що містить фільтри, параметри пагінації та сортування для ендпоінту GetAll.</typeparam>
/// <typeparam name="TGetDto">ДТО (вихідне) для ендпоінтів GetAll, GetById та Create.</typeparam>
/// <typeparam name="TCreateDto">ДТО (вхідне) для ендпоінту Create.</typeparam>
/// <typeparam name="TUpdateDto">ДТО (вхідне) для ендпоінту Update.</typeparam>
public abstract class EFApiControllerBase<TEntity, TKey, TGetAllArguments, TGetDto, TCreateDto, TUpdateDto> : ApiControllerBase<TEntity, TKey, TGetAllArguments, TGetDto, TCreateDto, TUpdateDto>
where TEntity : class, new()
where TKey : IEquatable<TKey>
where TGetAllArguments : IGetAllModel<TEntity>, new()
where TGetDto : new()
where TCreateDto : new()
where TUpdateDto : new()
{
    protected EFApiControllerBase(IEFEntityService<TEntity, TKey> service, IMapper mapper) : base(service, mapper)
    {
        _service = service;
    }

    protected abstract Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? Includes { get; }

    /// <summary>
    /// Отримати лист з ентіті.
    /// </summary>
    /// <param name="args">Об'єкт з фільтрами, параметрами пагінації та сортування.</param>
    /// <returns>200: Кортеж з двома параметрами: лист отриманних ентіті та загальна кількість знайдених ентіті (без пагинації).</returns>
    [HttpGet]
    public override async Task<IActionResult> GetAll([FromQuery] TGetAllArguments args)
    {
        var filters = args.GetFilters();
        var (orderBy, orderByDescending) = args.GetSorting();

        var (entities, totalCount) = await _service.GetAllWithTrackingAsync(
            includes: Includes,
            skip: args.Skip,
            take: args.Take,
            orderBy: orderBy,
            orderByDescending: orderByDescending,
            filters: filters);

        return Ok(new { entities = _mapper.Map<IEnumerable<TGetDto>>(entities), totalCount });
    }

    /// <summary>
    /// Отримати ентіті за айді.
    /// </summary>
    /// <param name="id">Айді ентіті</param>
    /// <returns>200: Знайдене ентіті. 404: Ентіті не знайдено.</returns>
    [HttpGet("{id}")]
    public override async Task<IActionResult> GetById(TKey id)
    {
        var entity = await _service.GetByIdWithTrackingAsync(
            id: id,
            includes: Includes);

        return entity != null ? Ok(_mapper.Map<TGetDto>(entity)) : NotFound();
    }

    protected new readonly IEFEntityService<TEntity, TKey> _service;
}

/// <summary>
/// Спрощений RESTlike абстрактний контроллер. TGetDto, TCreateDto та TUpdateDto дорівнюють TEntity.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TGetAllArguments"></typeparam>
public abstract class EFApiControllerBase<TEntity, TKey, TGetAllArguments> : EFApiControllerBase<TEntity, TKey, TGetAllArguments, TEntity, TEntity, TEntity>
where TEntity : class, new()
where TKey : IEquatable<TKey>
where TGetAllArguments : IGetAllModel<TEntity>, new()
{
    protected EFApiControllerBase(IEFEntityService<TEntity, TKey> service, IMapper mapper) : base(service, mapper) { }
}
