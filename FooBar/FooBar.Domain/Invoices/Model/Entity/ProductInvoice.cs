using FooBar.Domain.Common;
using FooBar.Domain.Products.Entity;

namespace FooBar.Domain.Invoices.Model.Entity
{
    public class ProductInvoice : DomainEntity
    {
        const decimal ValueIva = 0.19M;

        private decimal _quantity;
        private Product _product = default!;

        public required decimal Quantity
        {
            get => _quantity;
            set
            {
                value.ValidateGreaterThanZero("the quantity should be greater than zero.");
                _quantity = value;
            }
        }

        public Guid InvoiceId { get; set; }

        public required Product Product
        {
            get => _product;
            set
            {
                value.ValidateNull("the product should not be null.");
                _product = value;
            }
        }

        public decimal CalculateTotalWithIva() => CalculateSubTotal() + CalculateIva();

        private decimal CalculateIva()
        {
            decimal ivaCalculate = 0;
            if (Product.ApplyIva)
            {
                ivaCalculate = CalculateSubTotal() * ValueIva;
            }
            return ivaCalculate;
        }

        private decimal CalculateSubTotal() => Product.Value * Quantity;

    }
}
