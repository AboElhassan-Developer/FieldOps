using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FieldOps.IntegrationTests.Infrastructure;

namespace FieldOps.IntegrationTests.ApiConventions;

public sealed class ApiConventionsTests(FieldOpsApiFactory factory)
{
    private const string ProbeRoute = "/api/v1/conventions-probe";
    private const string ProblemContentType = "application/problem+json";

    [Fact]
    public async Task ValidRequest_PassesValidation()
    {
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            $"{ProbeRoute}/validation",
            new { name = "Acme", taxRate = 0.14m },
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task InvalidRequest_ReturnsValidationProblemWithFieldErrors()
    {
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            $"{ProbeRoute}/validation",
            new { name = "", taxRate = 14m },
            TestContext.Current.CancellationToken);

        using var problem = await ReadProblemAsync(response, HttpStatusCode.BadRequest);
        Assert.Equal("validation_failed", problem.RootElement.GetProperty("code").GetString());
        var errors = problem.RootElement.GetProperty("errors");
        Assert.True(errors.TryGetProperty("name", out _));
        Assert.True(errors.TryGetProperty("taxRate", out _));
    }

    [Fact]
    public async Task MalformedJson_ReturnsValidationProblem()
    {
        using var client = factory.CreateClient();
        using var content = new StringContent("{ \"name\": ", Encoding.UTF8, "application/json");

        using var response = await client.PostAsync(
            $"{ProbeRoute}/validation", content, TestContext.Current.CancellationToken);

        using var problem = await ReadProblemAsync(response, HttpStatusCode.BadRequest);
        Assert.Equal("validation_failed", problem.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task UnhandledException_ReturnsInternalErrorWithoutDetails()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"{ProbeRoute}/exception", TestContext.Current.CancellationToken);

        using var problem = await ReadProblemAsync(response, HttpStatusCode.InternalServerError);
        Assert.Equal("internal_error", problem.RootElement.GetProperty("code").GetString());
        var body = problem.RootElement.GetRawText();
        Assert.DoesNotContain(ConventionsProbeController.SensitiveDetail, body, StringComparison.Ordinal);
        Assert.DoesNotContain(nameof(InvalidOperationException), body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExpectedError_ReturnsMappedStatusAndCode()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"{ProbeRoute}/conflict", TestContext.Current.CancellationToken);

        using var problem = await ReadProblemAsync(response, HttpStatusCode.Conflict);
        Assert.Equal("probe_already_exists", problem.RootElement.GetProperty("code").GetString());
        Assert.Equal("The probe already exists.", problem.RootElement.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task UnknownRoute_ReturnsNotFoundProblem()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/v1/does-not-exist", TestContext.Current.CancellationToken);

        using var problem = await ReadProblemAsync(response, HttpStatusCode.NotFound);
        Assert.Equal("not_found", problem.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Enums_AreSerializedAsStrings()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"{ProbeRoute}/enum", TestContext.Current.CancellationToken);
        using var json = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Active", json.RootElement.GetProperty("status").GetString());
    }

    // Every problem response must use the ProblemDetails media type and carry a code and a trace id.
    private static async Task<JsonDocument> ReadProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(ProblemContentType, response.Content.Headers.ContentType?.MediaType);

        var problem = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.Equal((int)expectedStatus, problem.RootElement.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrEmpty(problem.RootElement.GetProperty("code").GetString()));
        Assert.False(string.IsNullOrEmpty(problem.RootElement.GetProperty("traceId").GetString()));
        return problem;
    }
}
