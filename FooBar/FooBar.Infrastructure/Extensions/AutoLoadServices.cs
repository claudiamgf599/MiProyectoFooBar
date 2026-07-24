using FooBar.Application.Invoice.Command.Factory;
using FooBar.Application.Ports;
using FooBar.Domain.Common;
using FooBar.Infrastructure.Adapters;
using FooBar.Infrastructure.Port;
using Microsoft.Extensions.DependencyInjection;

namespace FooBar.Infrastructure.Extensions;

public static class AutoLoadServices
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient(typeof(IRepository<>), typeof(GenericRepository<>));

        services.AddTransient(typeof(InvoiceFactory));

        services.AddTransient<IUnitOfWork, UnitOfWork>();

        var _services = AppDomain.CurrentDomain.GetAssemblies()
              .Where(assembly =>
              {
                  return assembly.FullName is null || assembly.FullName.Contains("Domain", StringComparison.OrdinalIgnoreCase);
              })
              .SelectMany(assembly => assembly.GetTypes())
              .Where(type => type.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(DomainServiceAttribute)));

        var _repositories = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly =>
            {
                return assembly.FullName is null || assembly.FullName.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase);
            })
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(RepositoryAttribute)));

        foreach (var service in _services)
        {
            services.AddTransient(service);
        }

        foreach (var repository in _repositories)
        {
            Type typeInterface = repository.GetInterfaces().Single();
            services.AddTransient(typeInterface, repository);
        }

        return services;
    }
}
