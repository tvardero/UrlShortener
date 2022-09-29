using Microsoft.AspNetCore.Identity;

namespace UrlShortener.DataAccess.DbEntities;

public class User : IdentityUser<Guid>
{
    public ICollection<ShortenedUrl> ShortenedUrls { get; init; } = new HashSet<ShortenedUrl>();

    [Required, MaxLength(64)]
    public override string UserName { get => base.UserName; set => base.UserName = value; }

    [Required, MaxLength(64), EmailAddress]
    public override string Email { get => base.Email; set => base.Email = value; }

    [Required, MaxLength(32), Phone]
    public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }
}