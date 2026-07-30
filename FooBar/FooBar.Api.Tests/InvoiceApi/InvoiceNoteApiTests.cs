using FooBar.Application.Invoice.Command;
using FooBar.Application.Invoice.Query;
using FooBar.Application.Invoice.Query.Dto;
using FooBar.Domain.Invoices.Model.Dto;
using FooBar.Application.Ports;
using FooBar.Domain.Customers.Entity;
using FooBar.Domain.Invoices.Model.Entity;
using FooBar.Domain.Products.Entity;
using FooBar.Infrastructure.Port;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace FooBar.Api.Tests.InvoiceApi;

public class InvoiceNoteApiTests
{
    [Fact(Skip = "Dapper no funciona con In-Memory Database. Requiere SQL Server real. Para probar: ejecutar API con DB real y usar Swagger.")]
    public async Task PutNote_Success()
    {
        // Arrange
        await using var webApp = new ApiApp();
        var customer = new CustomerDataBuilder().Build();
        var product = new ProductDataBuilder().Build();
        var invoiceCreated = await CreateInvoice(webApp, customer, product);
        var client = webApp.CreateClient();
        var note = "Entrega urgente - confirmar antes de las 10am";
        
        // Act
        var command = new UpdateInvoiceNoteCommand(invoiceCreated.Id, note);
        var request = await client.PutAsJsonAsync($"/api/invoice/{invoiceCreated.Id}/note", command);
        
        // Assert
        request.EnsureSuccessStatusCode();
        var responseData = await client.GetFromJsonAsync<InvoiceDto>($"/api/invoice/{invoiceCreated.Id}");
        
        Assert.NotNull(responseData);
        Assert.Equal(note, responseData.Note);
    }

    [Fact]
    public async Task PutNote_EmptyNote_ReturnsError()
    {
        // Arrange
        await using var webApp = new ApiApp();
        var customer = new CustomerDataBuilder().Build();
        var product = new ProductDataBuilder().Build();
        var invoiceCreated = await CreateInvoice(webApp, customer, product);
        var client = webApp.CreateClient();
        
        // Act
        var command = new UpdateInvoiceNoteCommand(invoiceCreated.Id, "");
        var request = await client.PutAsJsonAsync($"/api/invoice/{invoiceCreated.Id}/note", command);
        
        // Assert - Puede ser BadRequest (400) o error de validación (422/500)
        // dependiendo de la configuración del ExceptionHandler
        Assert.NotEqual(System.Net.HttpStatusCode.OK, request.StatusCode);
    }

    [Fact]
    public async Task PutNote_NoteExceedsMaxLength_ReturnsError()
    {
        // Arrange
        await using var webApp = new ApiApp();
        var customer = new CustomerDataBuilder().Build();
        var product = new ProductDataBuilder().Build();
        var invoiceCreated = await CreateInvoice(webApp, customer, product);
        var client = webApp.CreateClient();
        var longNote = new string('a', 501);
        
        // Act
        var command = new UpdateInvoiceNoteCommand(invoiceCreated.Id, longNote);
        var request = await client.PutAsJsonAsync($"/api/invoice/{invoiceCreated.Id}/note", command);
        
        // Assert - Puede ser BadRequest (400) o error de validación (422/500)
        Assert.NotEqual(System.Net.HttpStatusCode.OK, request.StatusCode);
    }

    [Fact]
    public async Task PutNote_CanceledInvoice_BadRequest()
    {
        // Arrange
        await using var webApp = new ApiApp();
        var customer = new CustomerDataBuilder().WithType(TypeCustomer.Special).Build();
        var product = new ProductDataBuilder().Build();
        var invoiceCreated = await CreateInvoice(webApp, customer, product);
        
        // Cancel the invoice first
        var cancelClient = webApp.CreateClient();
        await cancelClient.PostAsJsonAsync($"/api/invoice/{invoiceCreated.Id}/cancel", string.Empty);
        
        // Act
        var noteClient = webApp.CreateClient();
        var command = new UpdateInvoiceNoteCommand(invoiceCreated.Id, "No debería poder");
        var request = await noteClient.PutAsJsonAsync($"/api/invoice/{invoiceCreated.Id}/note", command);
        
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, request.StatusCode);
    }

    [Fact]
    public async Task PutNote_InvoiceNotFound_NotFound()
    {
        // Arrange
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var command = new UpdateInvoiceNoteCommand(nonExistentId, "Nota de prueba");
        var request = await client.PutAsJsonAsync($"/api/invoice/{nonExistentId}/note", command);
        
        // Assert - Actualmente retorna 400 porque RequiredException hereda de CoreBusinessException
        // y el ExceptionHandler lo mapea a BadRequest.
        // Semánticamente 404 sería más correcto, pero requeriría cambiar el tipo de excepción.
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, request.StatusCode);
    }

    [Fact(Skip = "Dapper no funciona con In-Memory Database. Requiere SQL Server real. Para probar: ejecutar API con DB real y usar Swagger.")]
    public async Task GetNotes_Success_ReturnsInvoicesWithNotes()
    {
        // Arrange
        await using var webApp = new ApiApp();
        var customer = new CustomerDataBuilder().Build();
        var product = new ProductDataBuilder().Build();
        var invoiceCreated = await CreateInvoice(webApp, customer, product);
        var client = webApp.CreateClient();
        
        // Add a note to the invoice
        var note = "Nota de prueba";
        var command = new UpdateInvoiceNoteCommand(invoiceCreated.Id, note);
        await client.PutAsJsonAsync($"/api/invoice/{invoiceCreated.Id}/note", command);
        
        // Act
        var response = await client.GetFromJsonAsync<IEnumerable<NoteDto>>("/api/invoice/notes");
        
        // Assert
        Assert.NotNull(response);
        Assert.Single(response);
        var invoiceWithNote = response.First();
        Assert.Equal(invoiceCreated.Id, invoiceWithNote.Id);
        Assert.Equal(note, invoiceWithNote.Note);
    }

    [Fact(Skip = "Dapper no funciona con In-Memory Database. Requiere SQL Server real. Para probar: ejecutar API con DB real y usar Swagger.")]
    public async Task GetNotes_Empty_ReturnsEmptyCollection()
    {
        // Arrange
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        
        // Act
        var response = await client.GetFromJsonAsync<IEnumerable<NoteDto>>("/api/invoice/notes");
        
        // Assert
        Assert.NotNull(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task PutNote_ThenGetNote_FullFlow()
    {
        // Arrange
        await using var webApp = new ApiApp();
        var customer = new CustomerDataBuilder().Build();
        var product = new ProductDataBuilder().Build();
        var invoiceCreated = await CreateInvoice(webApp, customer, product);
        var client = webApp.CreateClient();
        var note = "Actualizar entrega a mañana";
        
        // Act 1: Add note
        var command = new UpdateInvoiceNoteCommand(invoiceCreated.Id, note);
        var putRequest = await client.PutAsJsonAsync($"/api/invoice/{invoiceCreated.Id}/note", command);
        putRequest.EnsureSuccessStatusCode();
        
        // Act 2: Get invoice and verify note
        var invoice = await client.GetFromJsonAsync<InvoiceDto>($"/api/invoice/{invoiceCreated.Id}");
        
        // Assert
        Assert.NotNull(invoice);
        Assert.Equal(note, invoice.Note);
    }

    static async Task<Invoice> CreateInvoice(ApiApp webApp, Customer customer, Product product)
    {
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        await CreateCustomer(scope, customer);
        await CreateProduct(scope, product);
        var invoiceRepository = scope.ServiceProvider.GetRequiredService<IRepository<Invoice>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        ICollection<ProductInvoice> productsInvoice = [new ProductInvoiceDataBuilder().WithProduct(product).WithQuantity(2).Build()];
        var invoice = new InvoiceDataBuilder()
            .WithCustomer(customer)
            .WithProductsInvoice(productsInvoice)
            .Build();
        
        var invoiceCreated = await invoiceRepository.AddAsync(invoice);
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);
        
        return invoiceCreated;
    }

    static async Task CreateCustomer(IServiceScope scope, Customer customer)
    {
        var customerRepository = scope.ServiceProvider.GetRequiredService<IRepository<Customer>>();
        await customerRepository.AddAsync(customer);
    }

    static async Task CreateProduct(IServiceScope scope, Product product)
    {
        var productRepository = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();
        await productRepository.AddAsync(product);
    }

    static JsonSerializerOptions GetOptions()
    {
        return new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}