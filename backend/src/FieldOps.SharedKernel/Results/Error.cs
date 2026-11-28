namespace FieldOps.SharedKernel.Results;

/// <summary>
/// An expected business error. <see cref="Code"/> is stable and machine-readable
/// (e.g. <c>email_already_registered</c>); <see cref="Message"/> is for people.
/// </summary>
public sealed record Error(string Code, string Message, ErrorKind Kind)
{
    public static Error Validation(string code, string message) => new(code, message, ErrorKind.Validation);

    public static Error NotFound(string code, string message) => new(code, message, ErrorKind.NotFound);

    public static Error Conflict(string code, string message) => new(code, message, ErrorKind.Conflict);
}
