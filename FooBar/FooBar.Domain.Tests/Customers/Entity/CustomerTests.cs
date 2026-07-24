using FooBar.Domain.Exceptions;

namespace FooBar.Domain.Tests.Customers.Entity
{
    public class CustomerTests
    {
        const int MinimunLengthName = 3;
        const int MaximunLengthName = 100;

        [Fact]
        public void Customer_WithNameNull_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new CustomerDataBuilder().WithName(default!).Build());

            Assert.Equal("the name should not be null or empty.", exception.Message);
        }

        [Fact]
        public void Customer_WithNameLength_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new CustomerDataBuilder().WithName("Ad").Build());

            Assert.Equal($"the name should be between {MinimunLengthName} and {MaximunLengthName} characters.", exception.Message);
        }

        [Fact]
        public void Customer_WithTypeInvalid_RequiredException()
        {
            var exception = Assert.Throws<RequiredException>(() =>
                new CustomerDataBuilder().WithType((Domain.Customers.Entity.TypeCustomer)3).Build());

            Assert.Equal("the customer type is not valid.", exception.Message);
        }

        [Fact]
        public void Customer_IsPreferential_True()
        {
            var customer = new CustomerDataBuilder().WithType(Domain.Customers.Entity.TypeCustomer.Preferential).Build();

            Assert.True(customer.IsPreferential());
        }

        [Fact]
        public void Customer_IsCommon_True()
        {
            var customer = new CustomerDataBuilder().WithType(Domain.Customers.Entity.TypeCustomer.Common).Build();

            Assert.True(customer.IsCommon());
        }

        [Fact]
        public void Customer_IsSpecial_True()
        {
            var customer = new CustomerDataBuilder().WithType(Domain.Customers.Entity.TypeCustomer.Special).Build();

            Assert.True(customer.IsSpecial());
        }
    }
}
