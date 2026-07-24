using FooBar.Application.Invoice.Command;
using FooBar.Application.Invoice.Query.Dto;
using FooBar.Application.Ports;
using FooBar.Domain.Customers.Entity;
using FooBar.Domain.Invoices.Model.Entity;
using FooBar.Domain.Products.Entity;
using FooBar.Infrastructure.Port;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace FooBar.Api.Tests.InvoiceApi;

public class InvoiceApiTests
{

    [Fact]
    public async Task GetSingleClientsSuccess()
    {
        await using var webApp = new ApiApp();
        var customer = new CustomerDataBuilder().Build();
        var product = new ProductDataBuilder().Build();
        var invoiceCreated = await CreateInvoice(webApp, customer, product);
        var client = webApp.CreateClient();

        var singleInvoice = await client.GetFromJsonAsync<InvoiceDto>($"/api/invoice/{invoiceCreated.Id}");

        Assert.NotNull(singleInvoice);
        Assert.Equal(customer.Id, singleInvoice.Customer.Id);
        Assert.Equal(customer.Name, singleInvoice.Customer.Name);
        Assert.Single(singleInvoice.ProductsInvoice);
        var productInvoice = singleInvoice.ProductsInvoice.First();
        Assert.Equal(2, productInvoice.Quantity);
        Assert.Equal(product.Id, productInvoice.Product.Id);
        Assert.Equal(product.Name, productInvoice.Product.Name);
        Assert.Equal(product.ApplyIva, productInvoice.Product.ApplyIva);
        Assert.Equal(product.Value, productInvoice.Product.Value);
    }

    [Fact]
    public async Task PostClientsSuccess()
    {
        await using var webApp = new ApiApp();
        var serviceCollection = webApp.GetServiceCollection();
        using var scope = serviceCollection.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var customer = new CustomerDataBuilder().Build();
        var product = new ProductDataBuilder().Build();
        await CreateCustomer(scope, customer);
        await CreateProduct(scope, product);
        await unitOfWork.SaveAsync(new CancellationTokenSource().Token);
        InsertInvoiceCommand invoice = new InsertInvoiceCommandBuilder()
            .WithCustomerId(customer.Id)
            .WithProductsInvoice([new(product.Id, 2)])
            .Build();
        var client = webApp.CreateClient();

        var request = await client.PostAsJsonAsync("/api/invoice/", invoice);

        request.EnsureSuccessStatusCode();
        var invoiceId = JsonSerializer.Deserialize<Guid>(await request.Content.ReadAsStringAsync(), GetOptions());
        var responseData = await client.GetFromJsonAsync<InvoiceDto>($"/api/invoice/{invoiceId}");

        Assert.NotNull(responseData);
        Assert.NotEqual(Guid.Empty, responseData.Id);
        Assert.Equal(customer.Id, responseData.Customer.Id);
        Assert.Equal(238, responseData.ValueTotal);
    }

    [Fact]
    public async Task PostClientsSuccessCancel()
    {
        await using var webApp = new ApiApp();
        var customer = new CustomerDataBuilder().WithType(TypeCustomer.Special).Build();
        var product = new ProductDataBuilder().Build();
        var invoiceCreated = await CreateInvoice(webApp, customer, product);
        var client = webApp.CreateClient();

        var request = await client.PostAsJsonAsync($"/api/invoice/{invoiceCreated.Id}/cancel", string.Empty);

        request.EnsureSuccessStatusCode();
        var responseData = await client.GetFromJsonAsync<InvoiceDto>($"/api/invoice/{invoiceCreated.Id}");

        Assert.NotNull(responseData);
        Assert.Equal(InvoiceState.Canceled.ToString(), responseData.State);
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
