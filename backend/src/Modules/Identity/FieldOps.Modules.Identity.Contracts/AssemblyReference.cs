using System.Reflection;

namespace FieldOps.Modules.Identity.Contracts;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
