using FooBar.Domain.Common;

namespace FooBar.Domain.Products.Entity
{
    public class Product : DomainEntity
    {
        const int MinimunLengthName = 3;
        const int MaximunLengthName = 100;

        private string _name = default!;
        private decimal _value;

        public required string Name
        {
            get => _name;
            set
            {
                value.ValidateRequired("the name should not be null or empty.");
                value.ValidateLength(MinimunLengthName, MaximunLengthName, $"the name should be between {MinimunLengthName} and {MaximunLengthName} characters.");
                _name = value;
            }
        }

        public required bool ApplyIva { get; set; }

        public required decimal Value
        {
            get => _value;
            set
            {
                value.ValidateGreaterThanZero("the value should be greater than zero.");
                _value = value;
            }
        }

    }
}
