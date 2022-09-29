namespace UrlShortener.MVC.Models;

public interface IGetAllModel<TEntity>
where TEntity : class, new()
{
    public int Skip { get; init; }
    public int Take { get; init; }

    public Expression<Func<TEntity, bool>>[] GetFilters();

    public (Expression<Func<TEntity, object?>>? OrderBy, bool OrderByDescending) GetSorting();
}