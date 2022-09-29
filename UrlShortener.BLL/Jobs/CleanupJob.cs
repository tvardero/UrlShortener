using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UrlShortener.BLL.Jobs;

public class CleanupJob : BackgroundService
{
    public CleanupJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(async (_) => await Cleanup(), null, TimeSpan.Zero, TimeSpan.FromMinutes(10));

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Видалити всі адреса, в яких закінчився строк зберігання.
    /// </summary>
    /// <returns></returns>
    private async Task Cleanup()
    {
        const int batchSize = 200;

        using var scope = _serviceProvider.CreateScope();
        var entityService = scope.ServiceProvider.GetRequiredService<IEFEntityService<ShortenedUrl, string>>();

        var filters = new Expression<Func<ShortenedUrl, bool>>[]
        {
            e => e.ExpiredAtUtc <= DateTime.UtcNow
        };

        // Отримувати та оброблювати за раз не більше ніж batchSize.
        int totalCount;
        do
        {
            (var entities, totalCount) = await entityService.GetAllAsync(0, batchSize, e => e.Hash, false, filters);

            foreach (var e in entities)
            {
                entityService.RemoveWithNoSave(e);
            }

            await entityService.SaveAsync();

            await Task.Delay(500); // Зменшити навантаження на БД
        } while (totalCount > batchSize);
    }

    public override void Dispose()
    {
        _timer?.Dispose();

        GC.SuppressFinalize(this);
        base.Dispose();
    }

    private readonly IServiceProvider _serviceProvider;
    private Timer? _timer;
}