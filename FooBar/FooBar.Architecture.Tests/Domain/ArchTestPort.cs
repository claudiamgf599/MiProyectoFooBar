using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace ArchitectureTests
{
    public class ArchTestPort
    {
        private static readonly Architecture Architecture = new ArchLoader().LoadAssemblies(
            System.Reflection.Assembly.Load("FooBar.Domain")
        ).Build();

        [Fact]
        public void LosRepositoriosDebenTerminarConRepository()
        {
            var portNamespacePattern = "FooBar.Domain.*.Port";

            Interfaces()
            .That()
            .ResideInNamespace(portNamespacePattern, true)
            .Should()
            .HaveNameEndingWith("Repository")
            .Because("Los repositorios en la capa de dominio deben tener nombres que terminen con 'Repository'")
            .Check(Architecture);
        }
    }
}
