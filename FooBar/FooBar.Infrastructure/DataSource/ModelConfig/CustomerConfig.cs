using FooBar.Domain.Customers.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FooBar.Infrastructure.DataSource.ModelConfig
{
    internal class CustomerConfig : IEntityTypeConfiguration<Customer>
    {
        const int MaximunLengthName = 100;

        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(customer => customer.Id)
                .IsRequired();

            builder.Property(customer => customer.Name)
               .HasMaxLength(MaximunLengthName)
                .IsRequired();

            builder.Property(customer => customer.TypeCustomer)
               .HasColumnType("tinyint")
               .IsRequired();
        }
    }
}
