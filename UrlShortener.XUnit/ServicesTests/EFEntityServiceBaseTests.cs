namespace UrlShortener.XUnit.ServicesTests;

public class EFEntityServiceBaseTests
{
    static EFEntityServiceBaseTests()
    {
        _faker = new Faker<TestingEntity>("uk")
            .Rules((f, e) =>
            {
                e.Id = f.Random.Guid();
                e.Name = f.Name.FullName();
                e.Number = f.Random.Number(0, 1000);
            });

        _ctxMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
    }

    public EFEntityServiceBaseTests()
    {
        _data = _faker.GenerateBetween(50, 200);
        _ctxMock.Setup(m => m.Set<TestingEntity>()).ReturnsDbSet(_data);
        _service = new(_ctxMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsCorrect()
    {
        // Arrange
        var rng = new Random();

        var limit = rng.Next(0, 1000);
        var filters = new Expression<Func<TestingEntity, bool>>[] { e => e.Number >= limit };
        var orderByDescending = false;
        Expression<Func<TestingEntity, object?>> orderBy = e => e.Name;
        var skip = rng.Next(0, 50);
        var take = rng.Next(0, 50);
        var trackingBehavior = QueryTrackingBehavior.NoTracking;
        Expression<Func<TestingEntity, object>> selector = e => e.Name + e.Number;

        var query = _data.AsQueryable();
        foreach (var f in filters) query = query.Where(f);

        var expectedTotalCount = query.Count();

        query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        var expectedEntities = query.Select(selector).Skip(skip).Take(take).ToList();

        // Act
        var (actualEntities, actualTotalCount) = await _service.GetAllWithTrackingAsync<object>(selector, null, trackingBehavior, skip, take, orderBy, false, filters);

        // Assert
        Assert.Equal(expectedTotalCount, actualTotalCount);
        Assert.Equal(expectedEntities, actualEntities);
    }

    private static readonly Faker<TestingEntity> _faker;
    private static readonly Mock<AppDbContext> _ctxMock;
    private readonly List<TestingEntity> _data;
    private readonly TestedService _service;
}