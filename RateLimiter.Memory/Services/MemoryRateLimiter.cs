using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RateLimiter.Abstraction;
using RateLimiter.Abstraction.Services;

namespace RateLimiter.Memory.Services;

public class MemoryRateLimiter(IMemoryCache cache, IOptions<RateLimitationConfiguration> options) : IRateLimiter
{
    private readonly IMemoryCache _cache = cache;
    private static readonly ConcurrentDictionary<string, object> _locks = new();
    private readonly RateLimitationConfiguration _config = options.Value ?? new RateLimitationConfiguration();

    public bool IsRequestAllowed(string key)
    {
        return IsRequestAllowed(key, _config.Limit, _config.TimeWindow);
    }

    public bool IsRequestAllowed(string key, int limit, int timeWindow)
    {
        var lockObj = _locks.GetOrAdd(key, new object());

        lock (lockObj)
        {
            var requestCount = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(timeWindow);
                return 0;
            });

            if (requestCount < limit)
            {
                _cache.Set(key, requestCount + 1, TimeSpan.FromSeconds(timeWindow));
                return true;
            }

            return false;
        }

    }

}
