namespace RateLimiter.Abstraction.Services;

public interface IRateLimiter
{
    bool IsRequestAllowed(string key);
    bool IsRequestAllowed(string key, int limit, int timeWindowInSeconds);
}
