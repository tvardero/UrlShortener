namespace UrlShortener.MVC.Models;

public class RegisterModel
{
    [Required, MaxLength(64)]
    public string Username { get; init; } = null!;

    [Required, DataType(DataType.Password)]
    public string Password { get; init; } = null!;

    public bool RememberMe { get; init; } = false;

    [Required, MaxLength(64), EmailAddress]
    public string Email { get; init; } = null!;

    [Required, MaxLength(32), Phone]
    public string PhoneNumber { get; init; } = null!;

    [HiddenInput]
    public string? ReturnUrl { get; init; }
}