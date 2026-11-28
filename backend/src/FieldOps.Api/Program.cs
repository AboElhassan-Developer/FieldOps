using FieldOps.Modules.Identity;
using FieldOps.SharedKernel;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddIdentityModule();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Liveness: the process is running. Does not check dependencies, so a short
// database outage does not cause the host to restart the application.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });

// Readiness: the application can serve requests (database reachable).
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains(HealthCheckTags.Ready),
});

app.MapControllers();

app.Run();
