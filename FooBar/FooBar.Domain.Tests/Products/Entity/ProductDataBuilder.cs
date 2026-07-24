using FooBar.Domain.Products.Entity;

namespace FooBar.Domain.Tests.Products.Entity
{
    public class ProductDataBuilder
    {
        Guid _id = Guid.NewGuid();
        string _name = "One";
        bool _applyIva = true;
        decimal _value = 100;

        public ProductDataBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public ProductDataBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ProductDataBuilder WithApplyIva(bool applyIva)
        {
            _applyIva = applyIva;
            return this;
        }

        public ProductDataBuilder WithValue(decimal value)
        {
            _value = value;
            return this;
        }

        public Product Build()
        {
            return new Product()
            {
                Id = _id,
                Name = _name,
                ApplyIva = _applyIva,
                Value = _value
            };
        }
    }
}
