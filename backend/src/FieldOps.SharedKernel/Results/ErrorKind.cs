namespace FieldOps.SharedKernel.Results;

/// <summary>Category of an expected error; determines the HTTP status code returned to the client.</summary>
public enum ErrorKind
{
    /// <summary>The request breaks a business rule (400).</summary>
    Validation,

    /// <summary>The requested resource does not exist (404).</summary>
    NotFound,

    /// <summary>The request conflicts with the current state, e.g. a duplicate (409).</summary>
    Conflict,
}
