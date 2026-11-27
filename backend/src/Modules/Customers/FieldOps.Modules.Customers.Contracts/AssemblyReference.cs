using System.Reflection;

namespace FieldOps.Modules.Customers.Contracts;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
