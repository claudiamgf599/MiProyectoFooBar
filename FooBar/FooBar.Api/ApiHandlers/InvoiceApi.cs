using FooBar.Api.Filters;
using FooBar.Application.Invoice.Command;
using FooBar.Application.Invoice.Query;
using MediatR;

namespace FooBar.Api.ApiHandlers;

// En este archivo estático se definen los endpoints
public static class InvoiceApi
{
    public static RouteGroupBuilder MapInvoice(this IEndpointRouteBuilder routeHandler)
    {
        // POST /api/invoice/ — crear factura
        routeHandler.MapPost("/", async (IMediator mediator, [Validate] InsertInvoiceCommand invoice, HttpResponse response) =>
        {
            /*
            Cuando llamas mediator.Send(new InsertInvoiceCommand(...)), MediatR:

                Inspecciona el tipo del comando (InsertInvoiceCommand)
                Busca en DI el handler que implementa IRequestHandler<InsertInvoiceCommand, ?>
                Lo resuelve (con sus dependencias inyectadas)
                Ejecuta Handle(command, cancellationToken)
                Retorna el resultado
            */
            var invoiceId = await mediator.Send(invoice);
            var json = System.Text.Json.JsonSerializer.Serialize(invoiceId);
            response.ContentType = "application/json";
            await response.WriteAsync(json);
        })
       .Produces(statusCode: StatusCodes.Status201Created)
       .WithSummary("Create new invoice")
       .WithOpenApi();


        // POST /api/invoice/{id}/cancel — cancelar factura
        routeHandler.MapPost("/{id}/cancel", async (IMediator mediator, Guid id) =>
        {
            await mediator.Send(new CancelInvoiceCommand(id));
            return Results.Ok();
        })
       .Produces(statusCode: StatusCodes.Status200OK)
       .WithSummary("Cancel an invoice")
       .WithOpenApi();


        // GET /api/invoice/{id} — obtener por ID
        routeHandler.MapGet("/{id}", async (IMediator mediator, Guid id, HttpResponse response) =>
        {
            var result = await mediator.Send(new GetInvoiceByIdQuery(id));
            var json = System.Text.Json.JsonSerializer.Serialize(result);
            response.ContentType = "application/json";
            await response.WriteAsync(json);
        })
        .Produces(StatusCodes.Status200OK)
        .WithSummary("Get invoice by ID")
        .WithOpenApi();


        // GET /api/invoice/cancels — obtener canceladas
        routeHandler.MapGet("/cancels", async (IMediator mediator, HttpResponse response) =>
        {
            var result = await mediator.Send(new GetAllCancelQuery());
            var json = System.Text.Json.JsonSerializer.Serialize(result);
            response.ContentType = "application/json";
            await response.WriteAsync(json);
        })
        .Produces(StatusCodes.Status200OK)
        .WithSummary("Get canceled invoices")
        .WithOpenApi();

        return (RouteGroupBuilder)routeHandler;


        // TODO - 1 Registrar nuevo endpoint
    }
}
