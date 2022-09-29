namespace UrlShortener.MVC.Models;

public class UserModel
{
    [Required, MaxLength(64)]
    public string Username { get; init; } = null!;

    [Required, MaxLength(64), EmailAddress]
    public string Email { get; init; } = null!;

    [Required, MaxLength(32), Phone]
    public string PhoneNumber { get; init; } = null!;
}