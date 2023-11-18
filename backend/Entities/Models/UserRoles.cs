namespace Entities.Models;

public static class UserRoles
{
    public const string Administrator = "Administrator";
    public const string Receptionist = "Receptionist";

    public static readonly IReadOnlyCollection<string> All = new[] { Administrator, Receptionist };
}
