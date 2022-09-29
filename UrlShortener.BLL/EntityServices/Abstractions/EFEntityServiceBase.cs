using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace UrlShortener.BLL.EntityServices;

/// <summary>
/// Абстрактний клас сервісу з базовим функционалом для роботи з сутностями за допомогою Entity Framework.
/// </summary>
/// <typeparam name="TEntity">Тип сутності.</typeparam>
/// <typeparam name="TKey">Тип первинного ключа сутності.</typeparam>
public abstract class EFEntityServiceBase<TEntity, TKey> : EntityServiceBase<TEntity, TKey>, IEFEntityService<TEntity, TKey>, IEntityService<TEntity, TKey>
where TEntity : class, new()
where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Компарер для первинного ключа. Подвійна лямбда: спочатку айді, потім ентіті. Повертає булєвє значення чи співпадають айді ентіті та подане айді.
    /// </summary>
    /// <example>
    /// <c>TKey id => TEntity entity => entity.Id == id</c>
    /// </example>
    protected abstract Func<TKey, Expression<Func<TEntity, bool>>> PrimaryKeyComparer { get; }

    protected EFEntityServiceBase(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public virtual async Task<(IEnumerable<TResult> Entities, int TotalCount)> GetAllWithTrackingAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null)
    {
        var query = _ctx.Set<TEntity>().AsTracking(trackingBehavior);

        if (includes != null) foreach (var i in includes) query = i(query);
        if (filters != null) foreach (var f in filters) query = query.Where(f);

        var totalCount = await query.CountAsync();

        var entities = new List<TResult>();
        if (totalCount > 0)
        {
            if (orderBy != null) query = orderByDescending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);

            query = query.Skip(skip).Take(take);

            entities = await query.Select(selector).ToListAsync();
        }

        return (entities, totalCount);
    }

    public (IEnumerable<TResult> Entities, int TotalCount) GetAllWithTracking<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null)
    {
        return GetAllWithTrackingAsync(selector, includes, trackingBehavior, skip, take, orderBy, orderByDescending, filters).Result;
    }

    public async Task<(IEnumerable<TEntity> Entities, int TotalCount)> GetAllWithTrackingAsync(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null)
    {
        return await GetAllWithTrackingAsync(e => e, includes, trackingBehavior, skip, take, orderBy, orderByDescending, filters);
    }

    public (IEnumerable<TEntity> Entities, int TotalCount) GetAllWithTracking(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null)
    {
        return GetAllWithTrackingAsync(e => e, includes, trackingBehavior, skip, take, orderBy, orderByDescending, filters).Result;
    }

    public override async Task<(IEnumerable<TResult> Entities, int TotalCount)> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null)
    {
        return await GetAllWithTrackingAsync(selector, null, QueryTrackingBehavior.NoTrackingWithIdentityResolution, skip, take, orderBy, orderByDescending, filters);
    }

    public virtual async Task<TResult?> GetByIdWithTrackingAsync<TResult>(
        TKey id,
        Expression<Func<TEntity, TResult>> selector,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll)
    {
        var query = _ctx.Set<TEntity>().AsTracking(trackingBehavior);

        if (includes != null) foreach (var i in includes) query = i(query);

        return await query
            .Where(PrimaryKeyComparer(id))
            .Select(selector)
            .FirstOrDefaultAsync();
    }

    public TResult? GetByIdWithTracking<TResult>(
        TKey id,
        Expression<Func<TEntity, TResult>> selector,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll)
    {
        return GetByIdWithTrackingAsync(id, selector, includes, trackingBehavior).Result;
    }

    public async Task<TEntity?> GetByIdWithTrackingAsync(
        TKey id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll)
    {
        return await GetByIdWithTrackingAsync(id, e => e, includes, trackingBehavior);
    }

    public TEntity? GetByIdWithTracking(
        TKey id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll)
    {
        return GetByIdWithTrackingAsync(id, e => e, includes, trackingBehavior).Result;
    }

    public override async Task<TResult?> GetByIdAsync<TResult>(
        TKey id,
        Expression<Func<TEntity, TResult>> selector)
    where TResult : default
    {
        return await GetByIdWithTrackingAsync(id, selector, null, QueryTrackingBehavior.NoTrackingWithIdentityResolution);
    }

    public virtual async Task<int> SaveAsync()
    {
        return await _ctx.SaveChangesAsync();
    }

    public int Save()
    {
        return SaveAsync().Result;
    }

    public virtual EntityEntry<TEntity> CreateWithNoSave(TEntity entity)
    {
        return _ctx.Set<TEntity>().Add(entity);
    }

    public override async Task<TEntity> CreateAsync(TEntity entity)
    {
        var result = CreateWithNoSave(entity);
        await SaveAsync();

        return result.Entity;
    }

    public virtual EntityEntry<TEntity> UpdateWithNoSave(TEntity entity)
    {
        return _ctx.Update(entity);
    }

    public override async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var result = UpdateWithNoSave(entity);
        await SaveAsync();

        return result.Entity;
    }

    public override async Task RemoveAllAsync()
    {
        var entityTableType = _ctx.Model
            .GetEntityTypes()
            .Single(et => et.ClrType == typeof(TEntity));

        var tableName = $"[{entityTableType.GetSchema()}].[{entityTableType.GetTableName()}]";

        await _ctx.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE '{tableName}';");
    }

    public virtual EntityEntry<TEntity> RemoveWithNoSave(TEntity entity)
    {
        return _ctx.Remove(entity);
    }

    public override async Task<bool> RemoveAsync(TEntity entity)
    {
        RemoveWithNoSave(entity);

        return await SaveAsync() > 0;
        // Є баг. Якщо до виклику RemoveAsync() було зроблено інші зміни до контексту, але не збережено одразу,
        // то SaveAsync() > 0 верне true, навіть якщо видаленої ентіті не існувало в базі даних.
    }

    public virtual async Task<EntityEntry<TEntity>?> RemoveByIdWithNoSaveAsync(TKey id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return null;

        return RemoveWithNoSave(entity);
    }

    public virtual EntityEntry<TEntity>? RemoveByIdWithNoSave(TKey id)
    {
        var entity = GetByIdAsync(id).Result;
        if (entity == null) return null;

        return RemoveWithNoSave(entity);
    }

    protected readonly AppDbContext _ctx;
}