using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace ArchitectureTests
{
    public class ArchTestService
    {
        private static readonly Architecture Architecture = new ArchLoader().LoadAssemblies(
            System.Reflection.Assembly.Load("FooBar.Domain")
        ).Build();

        [Fact]
        public void LosServiciosDebenTerminarConService()
        {
            var portNamespacePattern = "FooBar.Domain.*.Service";

            Classes()
            .That()
            .ResideInNamespace(portNamespacePattern, true)
            .Should()
            .HaveNameEndingWith("Service")
            .Because("Los servicios en la capa de servicio deben tener nombres que terminen con 'Service'")
            .Check(Architecture);
        }
    }
}
