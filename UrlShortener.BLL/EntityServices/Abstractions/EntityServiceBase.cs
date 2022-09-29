namespace UrlShortener.BLL.EntityServices;

public abstract class EntityServiceBase<TEntity, TKey> : IEntityService<TEntity, TKey>
where TEntity : class, new()
where TKey : IEquatable<TKey>
{
    public abstract Task<(IEnumerable<TResult> Entities, int TotalCount)> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null);

    public (IEnumerable<TResult> Entities, int TotalCount) GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null)
    {
        return GetAllAsync(selector, skip, take, orderBy, orderByDescending, filters).Result;
    }

    public async Task<(IEnumerable<TEntity> Entities, int TotalCount)> GetAllAsync(
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null)
    {
        return await GetAllAsync(e => e, skip, take, orderBy, orderByDescending, filters);
    }

    public (IEnumerable<TEntity> Entities, int TotalCount) GetAll(
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null)
    {
        return GetAllAsync(e => e, skip, take, orderBy, orderByDescending, filters).Result;
    }

    public abstract Task<TResult?> GetByIdAsync<TResult>(
        TKey id,
        Expression<Func<TEntity, TResult>> selector);

    public TResult? GetById<TResult>(
        TKey id,
        Expression<Func<TEntity, TResult>> selector)
    {
        return GetByIdAsync(id, selector).Result;
    }

    public async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await GetByIdAsync(id, e => e);
    }

    public TEntity? GetById(TKey id)
    {
        return GetByIdAsync(id, e => e).Result;
    }

    public abstract Task<TEntity> CreateAsync(TEntity entity);

    public TEntity Create(TEntity entity)
    {
        return CreateAsync(entity).Result;
    }

    public abstract Task<TEntity> UpdateAsync(TEntity entity);

    public TEntity Update(TEntity entity)
    {
        return UpdateAsync(entity).Result;
    }

    public abstract Task RemoveAllAsync();

    public void RemoveAll()
    {
        RemoveAllAsync().Wait();
    }

    public abstract Task<bool> RemoveAsync(TEntity entity);

    public bool Remove(TEntity entity)
    {
        return RemoveAsync(entity).Result;
    }

    public async Task<bool> RemoveByIdAsync(TKey id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        return await RemoveAsync(entity);
    }

    public bool RemoveById(TKey id)
    {
        var entity = GetByIdAsync(id).Result;
        if (entity == null) return false;

        return RemoveAsync(entity).Result;
    }
}