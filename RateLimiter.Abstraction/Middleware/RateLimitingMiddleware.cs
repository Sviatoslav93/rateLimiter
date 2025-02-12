using Microsoft.AspNetCore.Http;
using RateLimiter.Abstraction.Services;

namespace RateLimiter.Abstraction.Middleware;

public class RateLimitingMiddleware(RequestDelegate next, IRateLimiter rateLimiter)
{
    private readonly RequestDelegate _next = next;
    private readonly IRateLimiter _rateLimiter = rateLimiter;

    public async Task InvokeAsync(HttpContext context)
    {
        var key = GetClientKey(context);

        if (!_rateLimiter.IsRequestAllowed(key))
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return;
        }

        await _next(context);
    }

    private static string GetClientKey(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
