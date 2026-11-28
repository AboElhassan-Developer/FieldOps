using System.Net;
using FieldOps.IntegrationTests.Infrastructure;

namespace FieldOps.IntegrationTests;

public sealed class HealthEndpointTests(FieldOpsApiFactory factory)
{
    [Theory]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    public async Task HealthEndpoint_WhenApplicationAndDatabaseAreUp_ReturnsHealthy(string path)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(path, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", body);
    }
}
