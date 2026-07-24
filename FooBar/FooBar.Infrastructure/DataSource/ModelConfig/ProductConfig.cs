using FooBar.Domain.Products.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FooBar.Infrastructure.DataSource.ModelConfig
{
    internal class ProductConfig : IEntityTypeConfiguration<Product>
    {
        const int MaximunLengthName = 100;

        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(product => product.Id)
                .IsRequired();

            builder.Property(product => product.Name)
                .HasMaxLength(MaximunLengthName)
                .IsRequired();

            builder.Property(product => product.Value)
                .HasColumnType("decimal(18,2)");
        }
    }
}
