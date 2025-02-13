using Microsoft.AspNetCore.Builder;
using RateLimiter.Abstraction.Middleware;

namespace RateLimiter.Abstraction;

public static class RateLimiterExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitingMiddleware>();
    }
}
