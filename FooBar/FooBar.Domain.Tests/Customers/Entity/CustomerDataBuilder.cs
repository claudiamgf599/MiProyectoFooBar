using FooBar.Domain.Customers.Entity;

namespace FooBar.Domain.Tests.Customers.Entity
{
    public class CustomerDataBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _name = "Custumer test";
        private TypeCustomer _typeCustomer = TypeCustomer.Special;

        public CustomerDataBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public CustomerDataBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public CustomerDataBuilder WithType(TypeCustomer typeCustomer)
        {
            _typeCustomer = typeCustomer;
            return this;
        }

        public Customer Build()
        {
            return new Customer()
            {
                Id = _id,
                Name = _name,
                TypeCustomer = _typeCustomer
            };
        }
    }
}
