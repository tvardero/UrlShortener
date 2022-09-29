using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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
[ApiController, Route("api/[controller]")]
public abstract class ApiControllerBase<TEntity, TKey, TGetAllArguments, TGetDto, TCreateDto, TUpdateDto> : ControllerBase
where TEntity : class, new()
where TKey : IEquatable<TKey>
where TGetAllArguments : IGetAllModel<TEntity>, new()
where TGetDto : new()
where TCreateDto : new()
where TUpdateDto : new()
{
    protected abstract Func<TEntity, TKey> PrimaryKeySelector { get; }

    protected ApiControllerBase(IEntityService<TEntity, TKey> service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Отримати лист з ентіті.
    /// </summary>
    /// <param name="args">Об'єкт з фільтрами, параметрами пагінації та сортування.</param>
    /// <returns>200: Кортеж з двома параметрами: лист отриманних ентіті та загальна кількість знайдених ентіті (без пагинації).</returns>
    [HttpGet]
    public virtual async Task<IActionResult> GetAll([FromQuery] TGetAllArguments args)
    {
        var filters = args.GetFilters();
        var (orderBy, orderByDescending) = args.GetSorting();

        var (entities, totalCount) = await _service.GetAllAsync(args.Skip, args.Take, orderBy, orderByDescending, filters);

        return Ok(new { entities = _mapper.Map<IEnumerable<TGetDto>>(entities), totalCount });
    }

    /// <summary>
    /// Отримати ентіті за айді.
    /// </summary>
    /// <param name="id">Айді ентіті</param>
    /// <returns>200: Знайдене ентіті. 404: Ентіті не знайдено.</returns>
    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetById(TKey id)
    {
        var entity = await _service.GetByIdAsync(id);

        return entity != null ? Ok(_mapper.Map<TGetDto>(entity)) : NotFound();
    }

    /// <summary>
    /// Створити нове ентіті.
    /// </summary>
    /// <param name="dto">ДТО для створення нового ентіті.</param>
    /// <returns>201: Створене ентіті та Location за яким його можна в подальшому отримати. 400: Помилки в ДТО.</returns>
    [HttpPost]
    public virtual async Task<IActionResult> Create(TCreateDto dto)
    {
        var entity = _mapper.Map<TEntity>(dto);
        var result = await _service.CreateAsync(entity);

        return CreatedAtAction(nameof(GetById), new { id = PrimaryKeySelector(result) }, _mapper.Map<TGetDto>(result));
    }

    /// <summary>
    /// Оновити повністю ентіті.
    /// </summary>
    /// <param name="id">Айді ентіті.</param>
    /// <param name="dto">ДТО для повного оновлення ентіті.</param>
    /// <returns>204: Ентіті оновлено. 400: Помилки в ДТО. 404: Ентіті не знайдено.</returns>
    [HttpPut("{id}")]
    public virtual async Task<IActionResult> UpdateById(TKey id, TUpdateDto dto)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        entity = _mapper.Map(dto, entity);

        await _service.UpdateAsync(entity);

        return NoContent();
    }

    /// <summary>
    /// Оновити частково ентіті.
    /// </summary>
    /// <param name="id">Айді ентіті.</param>
    /// <param name="patchDoc">Патч документ для часткового оновлення ентіті.</param>
    /// <returns>204: Ентіті оновлено. 400: Помилки при оновленні. 404: Ентіті не знайдено.</returns>

    [HttpPatch("{id}")]
    public virtual async Task<IActionResult> PatchById(TKey id, JsonPatchDocument<TEntity> patchDoc)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        patchDoc.ApplyTo(entity, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _service.UpdateAsync(entity);

        return NoContent();
    }

    /// <summary>
    /// Видалити всі ентіті типу <typeparamref name="TEntity"/> з бази даних.
    /// </summary>
    /// <param name="confirmRemovalOfAllEntities">Підтвердження видалення, щоб уникнути випадкового виклику ендпоінту (наприклад був намір видалити за айді).</param>
    /// <returns>204: Всі ентіті типу <typeparamref name="TEntity"/> видалено. 400: Підтвердження видалення не надіслано.</returns>
    [HttpDelete]
    public virtual async Task<IActionResult> DeleteAll(bool confirmRemovalOfAllEntities = false)
    {
        if (!confirmRemovalOfAllEntities) return BadRequest("To delete all entities in database, pass query param '?confirmRemovalOfAllEntities=true'");

        await _service.RemoveAllAsync();

        return NoContent();
    }

    /// <summary>
    /// Видалити ентіті за айді.
    /// </summary>
    /// <param name="id">Айді ентіті.</param>
    /// <returns>204: Ентіті видалено. 404: Ентіті не знайдено.</returns>
    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> DeleteById(TKey id)
    {
        var existed = await _service.RemoveByIdAsync(id);

        return existed ? NoContent() : NotFound();
    }

    protected readonly IEntityService<TEntity, TKey> _service;
    protected readonly IMapper _mapper;
}

/// <summary>
/// Спрощений RESTlike абстрактний контроллер. TGetDto, TCreateDto та TUpdateDto дорівнюють TEntity.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TGetAllArguments"></typeparam>
public abstract class ApiControllerBase<TEntity, TKey, TGetAllArguments> : ApiControllerBase<TEntity, TKey, TGetAllArguments, TEntity, TEntity, TEntity>
where TEntity : class, new()
where TKey : IEquatable<TKey>
where TGetAllArguments : IGetAllModel<TEntity>, new()
{
    protected ApiControllerBase(IEntityService<TEntity, TKey> service, IMapper mapper) : base(service, mapper) { }
}
