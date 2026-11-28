namespace FieldOps.SharedKernel.Web;

/// <summary>
/// Stable error codes returned in the <c>code</c> member of every ProblemDetails response.
/// Clients rely on these values, so they must never change once published.
/// </summary>
public static class ErrorCodes
{
    /// <summary>Name of the ProblemDetails extension member that carries the code.</summary>
    public const string ExtensionKey = "code";

    public const string ValidationFailed = "validation_failed";
    public const string BadRequest = "bad_request";
    public const string Unauthorized = "unauthorized";
    public const string Forbidden = "forbidden";
    public const string NotFound = "not_found";
    public const string MethodNotAllowed = "method_not_allowed";
    public const string Conflict = "conflict";
    public const string UnsupportedMediaType = "unsupported_media_type";
    public const string TooManyRequests = "too_many_requests";
    public const string InternalError = "internal_error";
    public const string Unknown = "error";
}
