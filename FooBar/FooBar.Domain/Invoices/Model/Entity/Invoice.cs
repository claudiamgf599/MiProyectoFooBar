using FooBar.Domain.Common;
using FooBar.Domain.Customers.Entity;
using FooBar.Domain.Exceptions;

namespace FooBar.Domain.Invoices.Model.Entity
{
    public class Invoice : DomainEntity
    {
        const decimal DiscountCustomerPreferential = 0.2M;
        const decimal DiscountCustomerSpecial = 0.1M;

        private Customer _customer = default!;
        private ICollection<ProductInvoice> _productsInvoice = default!;
        private InvoiceState _state;

        public required Customer Customer
        {
            get => _customer;
            set
            {
                value.ValidateNull("the customer should not be null.");
                _customer = value;
            }
        }

        public required ICollection<ProductInvoice> ProductsInvoice
        {
            get => _productsInvoice;
            set
            {
                value.ValidateNull("the products should not be null.");
                value.ValidateNotEmpty("the products should not be empty.");
                _productsInvoice = value;
                ValueTotal = CalculateValueTotal();
                ApplyDiscount();
            }
        }

        public decimal ValueTotal { get; private set; }

        public required InvoiceState State
        {
            get => _state;
            set
            {
                value.ValidateEnum("the invoice state is not valid.");
                _state = value;
            }
        }

        public void IsCancel()
        {
            if (State == InvoiceState.Canceled)
            {
                throw new CoreBusinessException("the invoice is already canceled.");
            }
        }

        public void Cancel()
        {
            if (Customer.IsCommon())
            {
                throw new CoreBusinessException("you cannot cancel the invoice of a common customer.");
            }
            State = InvoiceState.Canceled;
        }

        private void ApplyDiscount()
        {
            var discount = Customer.TypeCustomer switch
            {
                TypeCustomer.Preferential => ValueTotal * DiscountCustomerPreferential,
                TypeCustomer.Special => ValueTotal * DiscountCustomerSpecial,
                _ => 0
            };
            ValueTotal -= discount;
        }

        private decimal CalculateValueTotal()
        {
            decimal total = 0;
            foreach (var product in ProductsInvoice)
            {
                total += product.CalculateTotalWithIva();
            }
            return total;
        }
    }
}
