using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.BLL.CustomServices;

public class RangeProviderService
{
    /// <summary>
    /// Класс, що після використання автоматично звільняє діапазон лічільника та зберігає його значення в базі даних.
    /// </summary>
    public class CounterRangeBorrow : IDisposable
    {
        public CounterRange Range
        {
            get
            {
                ThrowIfDisposed();
                return _range;
            }
        }

        public CounterRangeBorrow(Action releaseAction, CounterRange range)
        {
            _releaseAction = releaseAction;
            _range = range;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException("This CounterRangeBorrow was disposed.");
        }

        public void Dispose()
        {
            if (_disposed) return;

            _releaseAction();

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~CounterRangeBorrow()
        {
            Dispose();
        }

        private bool _disposed = false;
        private readonly Action _releaseAction;
        private readonly CounterRange _range;
    }

    public const ulong RangeDistance = 1000000000;

    public RangeProviderService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        using var scope = serviceProvider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (ctx.CounterRanges.Any()) _counterRanges = GetRanges(ctx);
        else _counterRanges = SeedAndGetRanges(ctx);

        _semaphore = new(_counterRanges.Count, _counterRanges.Count);
    }

    public async Task<CounterRangeBorrow> GetFreeRange()
    {
        CounterRange? range = null;

        while (range == null)
        {
            await _semaphore.WaitAsync();
            lock (this)
            {
                range = _counterRanges.First(kv => kv.Value == false).Key;

                if (range.LastUsedValue >= HashGeneratorService.CombinationsAmount - 1
                    || (range.LastUsedValue >= range.RangeStart + RangeDistance - 1))
                {
                    _counterRanges.Remove(range);
                    range = null;
                }
            }
        }

        _counterRanges[range] = true;

        var borrow = new CounterRangeBorrow(() => ReleaseRange(range), range);
        return borrow;
    }

    public void ReleaseRange(CounterRange range)
    {
        if (!_counterRanges.ContainsKey(range)) throw new KeyNotFoundException();

        using var scope = _serviceProvider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ctx.Update(range);
        ctx.SaveChanges();

        _counterRanges[range] = false;
        _semaphore.Release();
    }

    private static Dictionary<CounterRange, bool> SeedAndGetRanges(AppDbContext ctx)
    {
        var result = new Dictionary<CounterRange, bool>();

        for (ulong i = 0; i < HashGeneratorService.CombinationsAmount; i += RangeDistance)
        {
            var range = new CounterRange()
            {
                RangeStart = i,
                LastUsedValue = i > 0 ? i - 1 : 0
            };
            ctx.Add(range);
            result.Add(range, false);
        }

        ctx.SaveChanges();

        return result;
    }

    private static Dictionary<CounterRange, bool> GetRanges(AppDbContext ctx)
    {
        var combinationsAmount = HashGeneratorService.CombinationsAmount;

        var result = ctx.CounterRanges
            .Where(r => r.LastUsedValue < combinationsAmount - 1)
            .Where(r => r.LastUsedValue < r.RangeStart + RangeDistance - 1)
            .ToDictionary(r => r, _ => false);

        if (!result.Any()) throw new ApplicationException("No vacant ranges found.");

        return result;
    }

    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<CounterRange, bool> _counterRanges; // true - занято
    private readonly SemaphoreSlim _semaphore;
}
