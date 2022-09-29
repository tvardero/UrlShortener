namespace UrlShortener.MVC.Models;

public class LoginModel
{
    [Required, MaxLength(64)]
    public string Username { get; init; } = null!;

    [Required, DataType(DataType.Password)]
    public string Password { get; init; } = null!;

    public bool RememberMe { get; init; } = false;

    [HiddenInput]
    public string? ReturnUrl { get; init; }
}