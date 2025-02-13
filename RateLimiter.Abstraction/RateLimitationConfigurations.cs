namespace RateLimiter.Abstraction;

public class RateLimitationConfiguration
{
    public int Limit { get; set; } = int.MaxValue;
    public int TimeWindow { get; set; } = int.MaxValue;
}
