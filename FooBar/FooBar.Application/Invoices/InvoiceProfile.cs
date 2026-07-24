using AutoMapper;
using FooBar.Application.Invoice.Query.Dto;

namespace FooBar.Application.Invoice
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<Domain.Invoices.Model.Entity.Invoice, InvoiceDto>();
            CreateMap<Domain.Customers.Entity.Customer, CustomerDto>();
            CreateMap<Domain.Invoices.Model.Entity.ProductInvoice, ProductInvoiceDto>();
            CreateMap<Domain.Products.Entity.Product, ProductDto>();
        }
    }
}
