namespace Velix.SDK;

public class VelixClientOptions
{
    public required string ApiUrl { get; init; }
    public required string ApiKey { get; init; }
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
    public int MaxRetries { get; init; } = 3;
}
