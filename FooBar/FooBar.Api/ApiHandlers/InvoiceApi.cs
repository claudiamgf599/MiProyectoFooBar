using FooBar.Api.Filters;
using FooBar.Application.Invoice.Command;
using FooBar.Application.Invoice.Query;
using MediatR;

namespace FooBar.Api.ApiHandlers;

public static class InvoiceApi
{
    public static RouteGroupBuilder MapInvoice(this IEndpointRouteBuilder routeHandler)
    {
        routeHandler.MapPost("/", async (IMediator mediator, [Validate] InsertInvoiceCommand invoice) =>
        {
            var invoiceId = await mediator.Send(invoice);
            return Results.Ok(invoiceId);
        })
       .Produces(statusCode: StatusCodes.Status201Created)
       .WithSummary("Create new invoice")
       .WithOpenApi();

        routeHandler.MapPost("/{id}/cancel", async (IMediator mediator, Guid id) =>
        {
            await mediator.Send(new CancelInvoiceCommand(id));
            return Results.Ok();
        })
       .Produces(statusCode: StatusCodes.Status200OK)
       .WithSummary("Cancel an invoice")
       .WithOpenApi();

        routeHandler.MapGet("/{id}", async (IMediator mediator, Guid id) =>
        {
            return Results.Ok(await mediator.Send(new GetInvoiceByIdQuery(id)));
        })
        .Produces(StatusCodes.Status200OK)
        .WithSummary("Get invoice by ID")
        .WithOpenApi();

        routeHandler.MapGet("/cancels", async (IMediator mediator) =>
        {
            return Results.Ok(await mediator.Send(new GetAllCancelQuery()));
        })
        .Produces(StatusCodes.Status200OK)
        .WithSummary("Get canceled invoices")
        .WithOpenApi();

        return (RouteGroupBuilder)routeHandler;

    }
}
