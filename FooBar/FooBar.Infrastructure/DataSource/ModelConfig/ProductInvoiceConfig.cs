using FooBar.Domain.Invoices.Model.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FooBar.Infrastructure.DataSource.ModelConfig
{
    internal class ProductInvoiceConfig : IEntityTypeConfiguration<ProductInvoice>
    {
        public void Configure(EntityTypeBuilder<ProductInvoice> builder)
        {
            builder.Property(productInvoice => productInvoice.Id)
                .IsRequired();

            builder.Property(productInvoice => productInvoice.Quantity)
                .HasColumnType("decimal(18,2)");
        }
    }
}
