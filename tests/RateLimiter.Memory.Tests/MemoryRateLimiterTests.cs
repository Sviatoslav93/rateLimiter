using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using RateLimiter.Abstraction;
using RateLimiter.Memory.Services;

namespace RateLimiter.Memory.Tests;

public class MemoryRateLimiterTests
{
    private readonly IMemoryCache _cache = Substitute.For<IMemoryCache>();
    private readonly IOptions<RateLimitationConfiguration> _options = Substitute.For<IOptions<RateLimitationConfiguration>>();

    private readonly MemoryRateLimiter _memoryRateLimiter;

    public MemoryRateLimiterTests()
    {
        SetupMemoryCache(_cache);
        _memoryRateLimiter = new MemoryRateLimiter(_cache, _options);
    }

    [Fact]
    public void Ratelimiter_AllowsRequest_WhenUnderLimit()
    {
        const int limit = 2;
        const int timeWindow = 60;

        var rateLimitationConfiguration = new RateLimitationConfiguration
        {
            Limit = limit,
            TimeWindow = timeWindow
        };

        _options.Value.Returns(rateLimitationConfiguration);

        var result = _memoryRateLimiter.IsRequestAllowed("test-key", limit, timeWindow);
        Assert.True(result);
    }

    [Fact]
    public void Ratelimiter_AllowsRequest_WhenUnderLimit_WithDifferentKeys()
    {
        const int limit = 2;
        const int timeWindow = 60;

        var rateLimitationConfiguration = new RateLimitationConfiguration
        {
            Limit = limit,
            TimeWindow = timeWindow
        };

        _options.Value.Returns(rateLimitationConfiguration);

        var result1 = _memoryRateLimiter.IsRequestAllowed("test-key-1", limit, timeWindow);
        Assert.True(result1);

        var result2 = _memoryRateLimiter.IsRequestAllowed("test-key-2", limit, timeWindow);
        Assert.True(result2);
    }

    private static void SetupMemoryCache(IMemoryCache cache)
    {
        var cacheEntries = new Dictionary<object, object>();

        object outValue;
        cache.TryGetValue(Arg.Any<string>(), out outValue!)
            .Returns(call =>
            {
                var key = call[0] as string;
                if (cacheEntries.TryGetValue(key!, out var cached))
                {
                    call[1] = cached;
                    return true;
                }
                return false;
            });

        cache.CreateEntry(Arg.Any<object>())
            .Returns(call =>
            {
                var entry = Substitute.For<ICacheEntry>();
                var key = call[0];
                entry.Key.Returns(key);

                entry.When(x => x.Value = Arg.Any<object>())
                     .Do(x => cacheEntries[key] = x.Arg<object>());

                return entry;
            });
    }
}
