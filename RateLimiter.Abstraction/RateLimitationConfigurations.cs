namespace RateLimiter.Abstraction;

public class RateLimitationConfigurations
{
    public int RateLimit { get; set; } = 100;
    public int TimeWindowInSeconds { get; set; } = 60;
}
