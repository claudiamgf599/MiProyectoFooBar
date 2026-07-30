using FooBar.Application.Invoice.Command;
using FooBar.Application.Ports;
using FooBar.Domain.Exceptions;
using FooBar.Domain.Invoices.Model.Entity;
using FooBar.Domain.Invoices.Port;
using FooBar.Domain.Tests.Invoices.Model.Entity;
using MediatR;
using NSubstitute;
using Xunit;

namespace FooBar.Application.Tests.Invoice.Command
{
    public class UpdateInvoiceNoteHandlerTests
    {
        [Fact]
        public async Task Handle_Success_SetsNoteAndSaves()
        {
            // Arrange
            var invoiceEntity = new InvoiceDataBuilder().Build();
            var repository = Substitute.For<IInvoiceRepository>();
            repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<string?>())
                .Returns(invoiceEntity);
            
            var unitOfWork = Substitute.For<IUnitOfWork>();
            
            var handler = new UpdateInvoiceNoteHandler(repository, unitOfWork);
            var command = new UpdateInvoiceNoteCommand(invoiceEntity.Id, "Entrega urgente");
            
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.Equal(Unit.Value, result);
            Assert.Equal("Entrega urgente", invoiceEntity.Note);
            await unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken?>());
        }

        [Fact]
        public async Task Handle_InvoiceNotFound_ThrowsException()
        {
            // Arrange
            var repository = Substitute.For<IInvoiceRepository>();
            repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<string?>())
                .Returns((Domain.Invoices.Model.Entity.Invoice?)null);
            
            var unitOfWork = Substitute.For<IUnitOfWork>();
            
            var handler = new UpdateInvoiceNoteHandler(repository, unitOfWork);
            var command = new UpdateInvoiceNoteCommand(Guid.NewGuid(), "Nota");
            
            // Act & Assert
            var exception = await Assert.ThrowsAsync<RequiredException>(() =>
                handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("the invoice does not exist.", exception.Message);
            await unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken?>());
        }

        [Fact]
        public async Task Handle_EmptyNote_ThrowsDomainException()
        {
            // Arrange
            var invoiceEntity = new InvoiceDataBuilder().Build();
            var repository = Substitute.For<IInvoiceRepository>();
            repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<string?>())
                .Returns(invoiceEntity);
            
            var unitOfWork = Substitute.For<IUnitOfWork>();
            
            var handler = new UpdateInvoiceNoteHandler(repository, unitOfWork);
            var command = new UpdateInvoiceNoteCommand(invoiceEntity.Id, "");
            
            // Act & Assert
            var exception = await Assert.ThrowsAsync<RequiredException>(() =>
                handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("the note should not be null or empty.", exception.Message);
            await unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken?>());
        }

        [Fact]
        public async Task Handle_NoteExceedsMaxLength_ThrowsDomainException()
        {
            // Arrange
            var invoiceEntity = new InvoiceDataBuilder().Build();
            var repository = Substitute.For<IInvoiceRepository>();
            repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<string?>())
                .Returns(invoiceEntity);
            
            var unitOfWork = Substitute.For<IUnitOfWork>();
            
            var handler = new UpdateInvoiceNoteHandler(repository, unitOfWork);
            var longNote = new string('a', 501);
            var command = new UpdateInvoiceNoteCommand(invoiceEntity.Id, longNote);
            
            // Act & Assert
            var exception = await Assert.ThrowsAsync<RequiredException>(() =>
                handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("the note should be between 1 and 500 characters.", exception.Message);
            await unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken?>());
        }

        [Fact]
        public async Task Handle_CanceledInvoice_ThrowsCoreBusinessException()
        {
            // Arrange
            var invoiceEntity = new InvoiceDataBuilder()
                .WithState(InvoiceState.Canceled)
                .Build();
            
            var repository = Substitute.For<IInvoiceRepository>();
            repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<string?>())
                .Returns(invoiceEntity);
            
            var unitOfWork = Substitute.For<IUnitOfWork>();
            
            var handler = new UpdateInvoiceNoteHandler(repository, unitOfWork);
            var command = new UpdateInvoiceNoteCommand(invoiceEntity.Id, "Nota");
            
            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(() =>
                handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("cannot add a note to a canceled invoice.", exception.Message);
            await unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken?>());
        }

        [Fact]
        public async Task Handle_TrimsSpaces_WhenSaving()
        {
            // Arrange
            var invoiceEntity = new InvoiceDataBuilder().Build();
            var repository = Substitute.For<IInvoiceRepository>();
            repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<string?>())
                .Returns(invoiceEntity);
            
            var unitOfWork = Substitute.For<IUnitOfWork>();
            
            var handler = new UpdateInvoiceNoteHandler(repository, unitOfWork);
            var command = new UpdateInvoiceNoteCommand(invoiceEntity.Id, "  Nota con espacios  ");
            
            // Act
            await handler.Handle(command, CancellationToken.None);
            
            // Assert
            Assert.Equal("Nota con espacios", invoiceEntity.Note);
        }

        [Fact]
        public async Task Handle_CancellationToken_PassedToRepository()
        {
            // Arrange
            var invoiceEntity = new InvoiceDataBuilder().Build();
            var repository = Substitute.For<IInvoiceRepository>();
            repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<string?>())
                .Returns(invoiceEntity);
            
            var unitOfWork = Substitute.For<IUnitOfWork>();
            
            var handler = new UpdateInvoiceNoteHandler(repository, unitOfWork);
            var cts = new CancellationTokenSource();
            var command = new UpdateInvoiceNoteCommand(invoiceEntity.Id, "Nota");
            
            // Act
            await handler.Handle(command, cts.Token);
            
            // Assert
            await repository.Received(1).GetByIdAsync(invoiceEntity.Id, Arg.Any<string?>());
            await unitOfWork.Received(1).SaveAsync(cts.Token);
        }
    }
}