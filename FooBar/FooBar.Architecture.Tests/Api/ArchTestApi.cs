using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using MediatR;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace ArchitectureTests
{
    public class ArchTestApi
    {
        private static readonly Architecture Architecture = new ArchLoader().LoadAssemblies(
            System.Reflection.Assembly.Load("FooBar.Api")
        ).Build();

        [Fact]
        public void Api_ShouldUseMediatorForRequests2()
        {
            var apiHandlerClasses = Classes()
                .That().ResideInNamespace("FooBar.Api.ApiHandlers", true)
                .And().DoNotHaveNameEndingWith("Validator");

            var rule = apiHandlerClasses.Should().DependOnAny(typeof(IMediator));
            rule.Check(Architecture);
        }


    }
}
