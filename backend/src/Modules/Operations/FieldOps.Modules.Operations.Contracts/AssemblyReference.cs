using System.Reflection;

namespace FieldOps.Modules.Operations.Contracts;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
