using FooBar.Domain.Invoices.Model.Entity;

namespace FooBar.Api.Tests.InvoiceApi
{
    public class ProductInvoiceDataBuilder
    {
        Guid _id = Guid.NewGuid();
        Domain.Products.Entity.Product _product = new ProductDataBuilder().Build();
        decimal _quantity = 3;

        public ProductInvoiceDataBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public ProductInvoiceDataBuilder WithProduct(Domain.Products.Entity.Product product)
        {
            _product = product;
            return this;
        }

        public ProductInvoiceDataBuilder WithQuantity(decimal quantity)
        {
            _quantity = quantity;
            return this;
        }

        public ProductInvoice Build()
        {
            return new ProductInvoice()
            {
                Id = _id,
                Product = _product!,
                Quantity = _quantity,
            };
        }
    }
}
