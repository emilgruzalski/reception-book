using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace ReceptionBook.Extensions;

public static class DataSeedingExtensions
{
    private const string AdministratorSection = "AdministratorAccount";

    public static async Task SeedAdministratorAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerManager>();
        var section = app.Configuration.GetSection(AdministratorSection);

        var userName = section["UserName"];
        var email = section["Email"];
        var password = section["Password"];

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarn("Administrator account is not configured. Skipping seeding.");
            return;
        }

        if (await userManager.FindByNameAsync(userName) is not null)
            return;

        var administrator = new User
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Administrator"
        };

        var result = await userManager.CreateAsync(administrator, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogError($"Failed to create administrator account: {errors}");
            return;
        }

        await userManager.AddToRoleAsync(administrator, UserRoles.Administrator);
        logger.LogInfo($"Administrator account '{userName}' has been created.");
    }
}
