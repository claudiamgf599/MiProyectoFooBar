using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using FooBar.Domain.Common;
using FooBar.Infrastructure.Adapters;

namespace ArchitectureTests
{
    public class PruebasDeArquitectura
    {

        private static readonly Architecture Architecture = new ArchLoader().LoadAssemblies(
            System.Reflection.Assembly.Load("FooBar.Api"),
            System.Reflection.Assembly.Load("FooBar.Domain"),
            System.Reflection.Assembly.Load("FooBar.Infrastructure"),
            System.Reflection.Assembly.Load("FooBar.Application")
        ).Build();

        private readonly IObjectProvider<IType> DomainLayer = Classes()
            .That()
            .ResideInNamespace("FooBar.Domain", true)
            .As("Domain Layer");

        private readonly IObjectProvider<IType> ApplicationLayer = Classes()
            .That()
            .ResideInNamespace("FooBar.Application", true)
            .As("Application Layer");

        private readonly IObjectProvider<IType> InfrastructureLayer = Classes()
            .That()
            .ResideInNamespace("FooBar.Infrastructure", true)
            .As("Infrastructure Layer");

        private readonly IObjectProvider<IType> ApiLayer = Classes()
            .That()
            .ResideInNamespace("FooBar.Api", true)
            .As("API Layer");

        [Fact]
        public void DomainLayer_ShouldNotDependOnApplicationLayer()
        {
            Types()
                .That()
                .Are(DomainLayer)
                .Should()
                .NotDependOnAny(ApplicationLayer)
                .Because("Las capas internas no deben depender de capas de aplicacion")
                .Check(Architecture);
        }

        [Fact]
        public void DomainLayer_ShouldNotDependOnInfrastructureLayer()
        {
            Types()
            .That()
            .Are(DomainLayer)
            .Should()
            .NotDependOnAny(InfrastructureLayer)
            .Because("Las capas internas no deben depender de capas de infraestructura")
            .Check(Architecture);
        }

        [Fact]
        public void DomainLayer_ShouldNotDependOnApiLayer()
        {
            Types()
                .That()
                .Are(DomainLayer)
                .Should()
                .NotDependOnAny(ApiLayer)
                .Because("Las capas internas no deben depender de la capa Api")
                .Check(Architecture);
        }

        [Fact]
        public void DomainLayer_ShouldNotDependOnExternalLibraries()
        {
            var rule = Classes()
                .That().ResideInNamespace("FooBar.Domain", true)
                .Should().NotDependOnAny("System.Net.Http")
                .AndShould().NotDependOnAny("Microsoft.EntityFrameworkCore");

            rule.Check(Architecture);
        }

        [Fact]
        public void Exceptions_ShouldInheritFromSystemException()
        {
            var rule = Classes()
                .That().ResideInNamespace("FooBar.Domain.Exceptions", true)
                .Should().BeAssignableTo(typeof(System.Exception));

            rule.Check(Architecture);
        }
    }
}