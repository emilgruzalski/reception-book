namespace Shared.RequestFeatures;

public class GuestParameters : RequestParameters
{
    public GuestParameters() => OrderBy = "lastName";

    public string? SearchTerm { get; set; }
}
