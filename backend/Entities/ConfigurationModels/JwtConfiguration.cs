namespace Entities.ConfigurationModels;

public class JwtConfiguration
{
    public string Section { get; set; } = "JwtSettings";

    public string? ValidIssuer { get; set; }
    public string? ValidAudience { get; set; }
    public string? SecretKey { get; set; }
    public int ExpiresInMinutes { get; set; }
    public int RefreshTokenExpiresInDays { get; set; }
}
