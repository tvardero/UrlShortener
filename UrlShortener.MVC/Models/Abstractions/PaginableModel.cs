namespace UrlShortener.MVC.Models;

public class PaginableModel<TEntity>
{
    public int CurrentSkip { get; init; }
    public int CurrentTake { get; init; }
    public IEnumerable<TEntity> Entities { get; init; } = null!;
    public int TotalCount { get; init; }
}