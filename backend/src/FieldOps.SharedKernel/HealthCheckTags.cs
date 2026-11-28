namespace FieldOps.SharedKernel;

public static class HealthCheckTags
{
    /// <summary>Checks that must pass before the application accepts traffic (e.g. database connectivity).</summary>
    public const string Ready = "ready";
}
