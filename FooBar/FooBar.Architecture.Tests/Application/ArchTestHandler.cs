using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace ArchitectureTests
{
    public class ArchTestHandler
    {
        private static readonly Architecture Architecture = new ArchLoader().LoadAssemblies(
            System.Reflection.Assembly.Load("FooBar.Application")
        ).Build();

        [Fact]
        public void LosManejadoresDebenEstarEnCommandYQuery()
        {
            var portNamespacePatternCommand = "FooBar.Application.*.Command";
            var portNamespacePatternQuery = "FooBar.Application.*.Query";

            Classes()
            .That()
            .ResideInNamespace(portNamespacePatternCommand, true)
            .Should()
            .HaveNameEndingWith("Handler")
            .OrShould()
            .HaveNameEndingWith("Command")
            .OrShould()
            .HaveNameEndingWith("Factory")
            .Because("Los manejadores deben estar en la capa de aplicaci�n y deben tener nombres que terminen con 'Handler'")
            .Check(Architecture);
        }
    }
}
