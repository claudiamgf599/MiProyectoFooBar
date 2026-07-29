using FluentValidation;
using System.Net;
using System.Reflection;

namespace FooBar.Api.Filters;

public static class ValidationFilter
{
    public static EndpointFilterDelegate ValidationFilterFactory(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
    {
        // 1. Busca todos los parámetros marcados con [Validate]
        IEnumerable<ValidationDescriptor> validationDescriptors = GetValidators(context.MethodInfo, context.ApplicationServices);

        // 2. Si hay validadores, envuelve el handler con validación
        if (validationDescriptors.Any())
        {
            return invocationContext => ValidateAsync(validationDescriptors, invocationContext, next);
        }

        // 3. Si no hay validadores, pasa directo al handler
        return invocationContext => next(invocationContext);
    }

    private static async ValueTask<object?> ValidateAsync(IEnumerable<ValidationDescriptor> validationDescriptors, EndpointFilterInvocationContext invocationContext, EndpointFilterDelegate next)
    {
        foreach (ValidationDescriptor descriptor in validationDescriptors)
        {
            var argument = invocationContext.Arguments[descriptor.ArgumentIndex];

            if (argument is not null)
            {
                // 4. Ejecuta el validator de FluentValidation
                var validationResult = await descriptor.Validator.ValidateAsync(
                    new ValidationContext<object>(argument)
                );

                // 5. Si falla → retorna 422
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary(),
                        statusCode: (int)HttpStatusCode.UnprocessableEntity);
                }
            }
        }

        // 6. Si todo pasa → ejecuta el handler original
        return await next.Invoke(invocationContext);
    }

    static IEnumerable<ValidationDescriptor> GetValidators(MethodBase methodInfo, IServiceProvider serviceProvider)
    {
        ParameterInfo[] parameters = methodInfo.GetParameters();

        for (int index = 0; index < parameters.Length; index++)
        {
            ParameterInfo parameter = parameters[index];

            // 7. Busca el atributo [Validate] en cada parámetro
            if (parameter.GetCustomAttribute<ValidateAttribute>() is not null)
            {
                // 8. Construye el tipo del validator: IValidator<InsertInvoiceCommand>
                Type validatorType = typeof(IValidator<>).MakeGenericType(parameter.ParameterType);

                // 9. Lo resuelve del DI container
                IValidator? validator = serviceProvider.GetService(validatorType) as IValidator;

                if (validator is not null)
                {
                    yield return new ValidationDescriptor { ArgumentIndex = index, ArgumentType = parameter.ParameterType, Validator = validator };
                }
            }
        }
    }

}
