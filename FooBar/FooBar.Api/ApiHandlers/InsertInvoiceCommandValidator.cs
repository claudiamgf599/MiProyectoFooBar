using FooBar.Application.Invoice.Command;
using FluentValidation;

namespace FooBar.Api.ApiHandlers;

public class InsertInvoiceCommandValidator : AbstractValidator<InsertInvoiceCommand>
{
    public InsertInvoiceCommandValidator()
    {
        RuleFor(command => command.CustomerId)
           .NotEmpty();

        RuleFor(command => command.ProductsInvoice)
            .NotNull()
            .NotEmpty();

        RuleForEach(command => command.ProductsInvoice)
            .SetValidator(new ProductInvoiceCommandValidator());
    }
}
