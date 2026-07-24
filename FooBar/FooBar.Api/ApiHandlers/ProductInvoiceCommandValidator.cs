using FooBar.Application.Invoice.Command;
using FluentValidation;

namespace FooBar.Api.ApiHandlers
{
    public class ProductInvoiceCommandValidator : AbstractValidator<ProductInvoiceCommand>
    {
        public ProductInvoiceCommandValidator()
        {
            RuleFor(command => command.ProductId)
                .NotEmpty();

            RuleFor(command => command.Quantity)
                .GreaterThan(default(int));
        }
    }
}
