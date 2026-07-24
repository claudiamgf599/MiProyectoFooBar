using FooBar.Domain.Common;

namespace FooBar.Domain.Customers.Entity
{
    public class Customer : DomainEntity
    {
        const int MinimunLengthName = 3;
        const int MaximunLengthName = 100;

        private string _name = default!;
        private TypeCustomer _typeCustomer;

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

        public required TypeCustomer TypeCustomer
        {
            get => _typeCustomer;
            set
            {
                value.ValidateEnum("the customer type is not valid.");
                _typeCustomer = value;
            }
        }

        public bool IsPreferential() => TypeCustomer == TypeCustomer.Preferential;

        public bool IsCommon() => TypeCustomer == TypeCustomer.Common;

        public bool IsSpecial() => TypeCustomer == TypeCustomer.Special;
    }
}
