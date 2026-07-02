namespace Velix.SDK.Exceptions;

public class RateLimitException : VelixException
{
    public int? RetryAfterSeconds { get; }

    public RateLimitException(int? retryAfterSeconds = null)
        : base("Rate limit exceeded", 429, "RATE_LIMIT_EXCEEDED")
    {
        RetryAfterSeconds = retryAfterSeconds;
    }
}
