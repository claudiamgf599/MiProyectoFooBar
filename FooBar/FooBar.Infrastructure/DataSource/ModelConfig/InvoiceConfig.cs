using FooBar.Domain.Invoices.Model.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FooBar.Infrastructure.DataSource.ModelConfig
{
    internal class InvoiceConfig : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.Property(invoice => invoice.Id)
                .IsRequired();

            builder.Property(invoice => invoice.State)
                .HasColumnType("tinyint")
                .IsRequired();

            builder.Property(invoice => invoice.ValueTotal)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(invoice => invoice.ProductsInvoice)
                .WithOne()
                .HasForeignKey(productInvoice => productInvoice.InvoiceId);
        }
    }
}
