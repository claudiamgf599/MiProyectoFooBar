using FooBar.Application.Invoice.Command;
using FluentValidation;

namespace FooBar.Api.Filters
{
    /// <summary>
    /// Validador para el command UpdateInvoiceNoteCommand.
    /// 
    /// Usa FluentValidation para definir reglas de validación declarativas.
    /// Este validador se registra automáticamente en DI y se conecta con el filtro [Validate].
    /// </summary>
    public class UpdateInvoiceNoteCommandValidator : AbstractValidator<UpdateInvoiceNoteCommand>
    {
        public UpdateInvoiceNoteCommandValidator()
        {
            // Validar que el ID no esté vacío
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("the invoice id is required.");
            
            // Validar que la nota no esté vacía
            RuleFor(x => x.Note)
                .NotEmpty()
                .WithMessage("the note cannot be empty.")
                .MaximumLength(500)
                .WithMessage("the note cannot exceed 500 characters.");
        }
    }
}