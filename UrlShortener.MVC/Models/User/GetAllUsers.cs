namespace UrlShortener.MVC.Models;

public class GetAllUsers : IGetAllModel<User>
{
    public enum OrderByEnum : byte
    {
        NotSpecified = 0,
        Id = NotSpecified,
        Username,
        Email,
        PhoneNumber
    }

    // Пагінація
    public int Skip { get; init; }
    public int Take { get; init; }

    // Сортування
    public OrderByEnum OrderBy { get; init; }
    public bool OrderByDescending { get; init; }

    // Фільтри
    public string? Username { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }

    public Expression<Func<User, bool>>[] GetFilters()
    {
        return new Expression<Func<User, bool>>[]
        {
            e => string.IsNullOrWhiteSpace(Username) || e.UserName.Contains(Username.Trim()),
            e => string.IsNullOrWhiteSpace(Email) || e.Email.Contains(Email.Trim()),
            e => string.IsNullOrWhiteSpace(PhoneNumber) || e.PhoneNumber.Contains(PhoneNumber.Trim()),
        };
    }

    public (Expression<Func<User, object?>>? OrderBy, bool OrderByDescending) GetSorting()
    {
        Expression<Func<User, object?>>? orderBy = OrderBy switch
        {
            OrderByEnum.Id => e => e.Id,
            OrderByEnum.Username => e => e.UserName,
            OrderByEnum.Email => e => e.Email,
            OrderByEnum.PhoneNumber => e => e.PhoneNumber,
            _ => null,
        };

        return (orderBy, OrderByDescending);
    }
}