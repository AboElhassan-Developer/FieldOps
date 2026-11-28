using FieldOps.IntegrationTests.Infrastructure;
using FieldOps.Modules.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FieldOps.IntegrationTests;

public sealed class DatabaseMigrationTests(FieldOpsApiFactory factory)
{
    [Fact]
    public async Task IdentityModule_AllMigrationsAreApplied()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityModuleDbContext>();

        var pending = await dbContext.Database.GetPendingMigrationsAsync(TestContext.Current.CancellationToken);

        Assert.Empty(pending);
    }

    [Fact]
    public async Task IdentityModule_ModelHasNoChangesMissingFromMigrations()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityModuleDbContext>();

        Assert.False(
            dbContext.Database.HasPendingModelChanges(),
            "The model has changed since the last migration. Add a new migration.");
    }
}
