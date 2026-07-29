using FooBar.Api.ApiHandlers;
using FooBar.Api.Filters;
using FooBar.Api.Middleware;
using FooBar.Infrastructure.DataSource;
using FooBar.Infrastructure.Extensions;
using FluentValidation;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;
using Serilog.Debugging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

/*builder.Services.AddDbContext<DataContext>(opts =>
{
    opts.UseSqlServer(config.GetConnectionString("db"));
});*/

builder.Services.AddDbContext<DataContext>(opts =>
{
    var useInMemory = config.GetValue<bool>("UseInMemoryDatabase");
    if (useInMemory)
    {
        opts.UseInMemoryDatabase("InMemoryDb");
    }
    else
    {
        opts.UseSqlServer(config.GetConnectionString("db"));
    }
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<DataContext>()
    .ForwardToPrometheus();

builder.Services.AddAutoMapper(Assembly.Load("FooBar.Application"));

builder.Services.AddServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Esto escanea automáticamente el ensamblado y encuentra:
/*
InsertInvoiceCommand          ──►  InsertInvoiceHandler : IRequestHandler<InsertInvoiceCommand, Guid>
CancelInvoiceCommand          ──►  CancelInvoiceHandler : IRequestHandler<CancelInvoiceCommand, Unit>
GetInvoiceByIdQuery           ──►  GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
*/

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("FooBar.Application"));
});

builder.Host.UseSerilog((_, loggerconfiguration) =>
    loggerconfiguration
        .WriteTo.Console()
        .WriteTo.File("logs.txt", Serilog.Events.LogEventLevel.Information));

SelfLog.Enable(Console.Error);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHttpMetrics();

app.UseMiddleware<AppExceptionHandlerMiddleware>();

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.UseRouting().UseEndpoints(endpoint =>
{
    endpoint.MapMetrics();
});

// Crea un grupo de endpoints con un prefijo común
// . MapInvoice llama al extension method definido en InvoiceApi. Registra todos los MapPost/MapGet dentro del grupo
// .AddEndpointFilterFactory agrega un filtro que se ejecuta antes de cada handler
// .WithTags("Invoices") — agrupa estos endpoints en Swagger bajo la etiqueta "Invoices"
app.MapGroup("/api/invoice")
    .MapInvoice()
    .AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory)
    .WithTags("Invoices");

app.Seed();

app.Run();
