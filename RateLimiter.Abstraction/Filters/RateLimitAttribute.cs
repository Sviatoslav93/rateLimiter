using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Abstraction.Services;

namespace RateLimiter.Abstraction.Filters;

public class RateLimitAttribute : ActionFilterAttribute
{
    private readonly int _limit;
    private readonly int _timeWindowInSeconds;

    public RateLimitAttribute(int limit, int timeWindowInSeconds)
    {
        _limit = limit;
        _timeWindowInSeconds = timeWindowInSeconds;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var rateLimiter = context.HttpContext.RequestServices.GetRequiredService<IRateLimiter>();
        var key = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (!rateLimiter.IsRequestAllowed(key, _limit, _timeWindowInSeconds))
        {
            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status429TooManyRequests,
                Content = "Too many requests. Please try again later."
            };
        }
    }
}
