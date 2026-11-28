using FieldOps.SharedKernel.Results;
using FieldOps.SharedKernel.Web;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.IntegrationTests.ApiConventions;

/// <summary>
/// Test-only endpoints that exercise the API conventions. Registered by the test host only;
/// never part of the application.
/// </summary>
[Route(ApiRoutes.V1 + "/[controller]")]
public sealed class ConventionsProbeController : ApiControllerBase
{
    public const string SensitiveDetail = "Sensitive internal detail";

    [HttpPost("validation")]
    public IActionResult Validate(ProbeRequest request) => Ok(request);

    [HttpGet("exception")]
    public IActionResult ThrowException() => throw new InvalidOperationException(SensitiveDetail);

    [HttpGet("conflict")]
    public IActionResult ReturnConflict()
    {
        Result result = Error.Conflict("probe_already_exists", "The probe already exists.");
        return result.IsSuccess ? NoContent() : Problem(result.Error);
    }

    [HttpGet("enum")]
    public IActionResult ReturnEnum() => Ok(new ProbeResponse(ProbeStatus.Active));
}

public sealed record ProbeRequest(string Name, decimal TaxRate);

public sealed record ProbeResponse(ProbeStatus Status);

public enum ProbeStatus
{
    Inactive,
    Active,
}

public sealed class ProbeRequestValidator : AbstractValidator<ProbeRequest>
{
    public ProbeRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty();
        RuleFor(request => request.TaxRate).GreaterThanOrEqualTo(0).LessThan(1);
    }
}
