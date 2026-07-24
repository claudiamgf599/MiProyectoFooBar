using FooBar.Domain.Exceptions;

namespace FooBar.Domain.Tests.Invoices.Model.Entity
{
    public class ProductInvoiceTests
    {
        [Fact]
        public void ProductInvoice_WithQuantityZero_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new ProductInvoiceDataBuilder().WithQuantity(default).Build());

            Assert.Equal("the quantity should be greater than zero.", exception.Message);
        }

        [Fact]
        public void ProductInvoice_WithProductNull_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new ProductInvoiceDataBuilder().WithProduct(default!).Build());

            Assert.Equal("the product should not be null.", exception.Message);
        }

        [Fact]
        public void ProductInvoice_CalculateTotalWithIva_Success()
        {
            var productInvoice = new ProductInvoiceDataBuilder().Build();

            var exception = Record.Exception(() => productInvoice.CalculateTotalWithIva());
            Assert.Null(exception);

            var totalWithIva = productInvoice.CalculateTotalWithIva();
            Assert.Equal(357, totalWithIva);
        }
    }
}
