using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.DataAccess.DbEntities;

namespace UrlShortener.DataAccess;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<ShortenedUrl> ShortenedUrls { get; init; } = null!;
    public DbSet<CounterRange> CounterRanges { get; init; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var shortenedUrl = builder.Entity<ShortenedUrl>();
        shortenedUrl.HasOne(su => su.User).WithMany(u => u.ShortenedUrls).OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(builder);
    }
}