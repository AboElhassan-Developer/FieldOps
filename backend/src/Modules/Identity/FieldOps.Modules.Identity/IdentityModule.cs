using FieldOps.Modules.Identity.Infrastructure.Persistence;
using FieldOps.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FieldOps.Modules.Identity;

public static class IdentityModule
{
    public const string ConnectionStringName = "FieldOps";

    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        services.AddDbContext<IdentityModuleDbContext>((serviceProvider, options) =>
        {
            var connectionString = serviceProvider.GetRequiredService<IConfiguration>()
                .GetConnectionString(ConnectionStringName)
                ?? throw new InvalidOperationException(
                    $"Connection string '{ConnectionStringName}' is not configured.");

            options.UseSqlServer(connectionString, sql => sql
                .MigrationsHistoryTable(HistoryRepository.DefaultTableName, IdentityModuleDbContext.Schema)
                .EnableRetryOnFailure());
        });

        services.AddHealthChecks()
            .AddDbContextCheck<IdentityModuleDbContext>("identity-database", tags: [HealthCheckTags.Ready]);

        return services;
    }
}
