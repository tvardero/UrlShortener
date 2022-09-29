using UrlShortener.BLL.EntityServices;

namespace UrlShortener.XUnit.ServicesTests;

internal class TestedService : EFEntityServiceBase<TestingEntity, Guid>
{
    public TestedService(AppDbContext ctx) : base(ctx) { }

    protected override Func<Guid, Expression<Func<TestingEntity, bool>>> PrimaryKeyComparer => id => e => e.Id == id;
}
