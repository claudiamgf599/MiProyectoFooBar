using FooBar.Domain.Exceptions;
using FooBar.Domain.Invoices.Port;
using FooBar.Domain.Invoices.Service;
using FooBar.Domain.Tests.Customers.Entity;
using FooBar.Domain.Tests.Invoices.Model.Entity;
using NSubstitute;

namespace FooBar.Domain.Tests.Invoices.Service
{
    public class CancelInvoiceServiceTests
    {
        readonly IInvoiceRepository _invoiceRepository;
        readonly CancelInvoiceService _cancelInvoiceService;

        public CancelInvoiceServiceTests()
        {
            _invoiceRepository = Substitute.For<IInvoiceRepository>();
            _cancelInvoiceService = new CancelInvoiceService(_invoiceRepository);
        }

        [Fact]
        public async Task ExecuteAsync_Success()
        {
            var invoice = new InvoiceDataBuilder().Build();
            _invoiceRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(invoice);
            await _cancelInvoiceService.ExecuteAsync(invoice.Id);

            await _invoiceRepository.Received().GetByIdAsync(Arg.Is(invoice.Id), "Customer");
            _invoiceRepository.Received(1).Update(Arg.Is(invoice));
            Assert.Equal(Domain.Invoices.Model.Entity.InvoiceState.Canceled, invoice.State);
        }

        [Fact]
        public async Task ExecuteAsync_InvoiceNotExist_RequiredException()
        {
            var invoice = new InvoiceDataBuilder().Build();

            var exception = await Assert.ThrowsAsync<RequiredException>(() =>
                _cancelInvoiceService.ExecuteAsync(invoice.Id));

            Assert.Equal("the invoice not exist.", exception.Message);
        }

        [Fact]
        public async Task ExecuteAsync_InvoiceIsCancel_RequiredException()
        {
            var invoice = new InvoiceDataBuilder()
                .WithState(Domain.Invoices.Model.Entity.InvoiceState.Canceled)
                .Build();
            _invoiceRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(invoice);

            var exception = await Assert.ThrowsAsync<CoreBusinessException>(() =>
                _cancelInvoiceService.ExecuteAsync(invoice.Id));

            Assert.Equal("the invoice is already canceled.", exception.Message);
        }

        [Fact]
        public async Task ExecuteAsync_CustomerIsCommon_RequiredException()
        {
            var invoice = new InvoiceDataBuilder()
                .WithCustomer(new CustomerDataBuilder().WithType(Domain.Customers.Entity.TypeCustomer.Common).Build())
                .Build();
            _invoiceRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(invoice);

            var exception = await Assert.ThrowsAsync<CoreBusinessException>(() =>
                _cancelInvoiceService.ExecuteAsync(invoice.Id));

            Assert.Equal("you cannot cancel the invoice of a common customer.", exception.Message);
        }
    }
}
