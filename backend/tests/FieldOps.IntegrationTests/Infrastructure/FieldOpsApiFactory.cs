using FieldOps.IntegrationTests.Infrastructure;
using FieldOps.Modules.Identity;
using FluentValidation;
using FieldOps.Modules.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(FieldOpsApiFactory))]

namespace FieldOps.IntegrationTests.Infrastructure;

/// <summary>
/// Hosts the API in memory against a disposable SQL Server container.
/// One instance (and one container) is shared by all tests in the assembly.
/// </summary>
public sealed class FieldOpsApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2025-latest";

    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder(SqlServerImage).Build();

    private string ConnectionString => new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
    {
        InitialCatalog = "FieldOps",
    }.ConnectionString;

    public async ValueTask InitializeAsync()
    {
        await _sqlServer.StartAsync();

        await using var scope = Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IdentityModuleDbContext>().Database.MigrateAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _sqlServer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // A dedicated environment: development-only configuration such as
        // user secrets is not loaded, so tests never reach a local database.
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(configuration => configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [$"ConnectionStrings:{IdentityModule.ConnectionStringName}"] = ConnectionString,
            }));

        // Test-only controllers and validators (e.g. the API conventions probe).
        builder.ConfigureTestServices(services =>
        {
            services.AddControllers().AddApplicationPart(typeof(FieldOpsApiFactory).Assembly);
            services.AddValidatorsFromAssemblyContaining<FieldOpsApiFactory>();
        });
    }
}
