namespace Entities.ConfigurationModels;

public class RateLimitingConfiguration
{
    public const string Section = "RateLimiting";

    public int AuthenticationPermitLimit { get; set; } = 5;
    public int AuthenticationWindowInSeconds { get; set; } = 60;
    public int GlobalPermitLimit { get; set; } = 100;
    public int GlobalWindowInSeconds { get; set; } = 60;
}
