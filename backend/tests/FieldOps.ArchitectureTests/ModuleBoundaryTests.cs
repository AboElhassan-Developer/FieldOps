using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using Assembly = System.Reflection.Assembly;

namespace FieldOps.ArchitectureTests;

public sealed class ModuleBoundaryTests
{
    private static readonly Dictionary<string, Assembly> ModuleAssemblies = new()
    {
        ["Identity"] = Modules.Identity.AssemblyReference.Assembly,
        ["Customers"] = Modules.Customers.AssemblyReference.Assembly,
        ["Operations"] = Modules.Operations.AssemblyReference.Assembly,
        ["Billing"] = Modules.Billing.AssemblyReference.Assembly,
    };

    private static readonly Dictionary<string, Assembly> ContractAssemblies = new()
    {
        ["Identity"] = Modules.Identity.Contracts.AssemblyReference.Assembly,
        ["Customers"] = Modules.Customers.Contracts.AssemblyReference.Assembly,
        ["Operations"] = Modules.Operations.Contracts.AssemblyReference.Assembly,
        ["Billing"] = Modules.Billing.Contracts.AssemblyReference.Assembly,
    };

    private static readonly Assembly SharedKernelAssembly = SharedKernel.AssemblyReference.Assembly;

    private static readonly Assembly ApiAssembly = typeof(Program).Assembly;

    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
        [
            .. ModuleAssemblies.Values,
            .. ContractAssemblies.Values,
            SharedKernelAssembly,
            ApiAssembly,
        ])
        .Build();

    public static TheoryData<string> ModuleNames => [.. ModuleAssemblies.Keys];

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Module_ShouldNotDependOnOtherModulesImplementation(string module)
    {
        var otherModules = ModuleAssemblies
            .Where(m => m.Key != module)
            .Select(m => m.Value)
            .ToArray();

        Types().That().ResideInAssembly(ModuleAssemblies[module])
            .Should().NotDependOnAny(Types().That().ResideInAssembly(otherModules[0], otherModules[1..]))
            .Because("modules communicate only through the public contracts of other modules")
            .Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Contracts_ShouldNotDependOnAnyModuleImplementation(string module)
    {
        var allModules = ModuleAssemblies.Values.ToArray();

        Types().That().ResideInAssembly(ContractAssemblies[module])
            .Should().NotDependOnAny(Types().That().ResideInAssembly(allModules[0], allModules[1..]))
            .Because("contracts are the public surface of a module and must not expose its internals")
            .Check(Architecture);
    }

    [Fact]
    public void SharedKernel_ShouldNotDependOnModulesOrApi()
    {
        Assembly[] forbidden = [.. ModuleAssemblies.Values, .. ContractAssemblies.Values, ApiAssembly];

        Types().That().ResideInAssembly(SharedKernelAssembly)
            .Should().NotDependOnAny(Types().That().ResideInAssembly(forbidden[0], forbidden[1..]))
            .Because("the shared kernel holds building blocks used by every module and depends on none of them")
            .Check(Architecture);
    }

    [Fact]
    public void ModulesAndContracts_ShouldNotDependOnApi()
    {
        Assembly[] modulesAndContracts = [.. ModuleAssemblies.Values, .. ContractAssemblies.Values];

        Types().That().ResideInAssembly(modulesAndContracts[0], modulesAndContracts[1..])
            .Should().NotDependOnAny(Types().That().ResideInAssembly(ApiAssembly))
            .Because("the API host composes the modules, never the other way around")
            .Check(Architecture);
    }
}
