using Microsoft.EntityFrameworkCore;
using Repository;

namespace ReceptionBook.Extensions;

public static class DatabaseExtensions
{
    private const string MigrateOnStartupKey = "Database:MigrateOnStartup";

    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>(MigrateOnStartupKey))
            return;

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RepositoryContext>();

        await context.Database.MigrateAsync();
    }
}
