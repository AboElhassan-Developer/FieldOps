using FieldOps.SharedKernel.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FieldOps.SharedKernel.Web;

/// <summary>Base class for API controllers in every module.</summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Converts an expected business error into a ProblemDetails response.</summary>
    protected ObjectResult Problem(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        var statusCode = error.Kind switch
        {
            ErrorKind.Validation => StatusCodes.Status400BadRequest,
            ErrorKind.NotFound => StatusCodes.Status404NotFound,
            ErrorKind.Conflict => StatusCodes.Status409Conflict,
            _ => throw new ArgumentOutOfRangeException(nameof(error), error.Kind, "Unsupported error kind."),
        };

        var problem = ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode, detail: error.Message);
        problem.Extensions[ErrorCodes.ExtensionKey] = error.Code;

        return new ObjectResult(problem) { StatusCode = statusCode };
    }
}
