namespace UrlShortener.MVC.Models;

public class ChangePasswordModel
{
    [Required]
    public string OldPassword { get; init; } = null!;

    [Required]
    public string NewPassword { get; init; } = null!;
}