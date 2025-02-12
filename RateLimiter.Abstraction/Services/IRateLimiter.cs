namespace RateLimiter.Abstraction.Services;

public interface IRateLimiter
{
    bool IsRequestAllowed(string key);
}
