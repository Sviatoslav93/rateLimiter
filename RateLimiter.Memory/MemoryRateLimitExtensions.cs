using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Abstraction;
using RateLimiter.Abstraction.Services;
using RateLimiter.Memory.Services;

namespace RateLimiter.Memory;

public static class MemoryRateLimitExtensions
{
    public static IServiceCollection AddMemoryRateLimiter(this IServiceCollection services, Action<RateLimitationConfiguration>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }

        services.AddMemoryCache();
        services.AddSingleton<IRateLimiter, MemoryRateLimiter>();

        return services;
    }
}
