namespace UrlShortener.MVC.Models;

public class GetAllUsers : IGetAllModel<User>
{
    public enum OrderByEnum : byte
    {

    }

    // Пагінація
    public int Skip { get; init; }
    public int Take { get; init; }

    // Сортування

    // Фільтри

    public Expression<Func<User, bool>>[] GetFilters()
    {
        throw new NotImplementedException();
    }

    public (Expression<Func<User, object?>>? OrderBy, bool OrderByDescending) GetSorting()
    {
        throw new NotImplementedException();
    }
}