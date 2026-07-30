using FooBar.Application.Invoice.Query;
using FooBar.Domain.Invoices.Model.Dto;
using FooBar.Domain.Invoices.Port;
using NSubstitute;
using Xunit;

namespace FooBar.Application.Tests.Invoice.Query
{
    public class GetNotesHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsNotesFromRepository()
        {
            // Arrange
            var expectedNotes = new List<NoteDto>
            {
                new NoteDto
                {
                    Id = Guid.NewGuid(),
                    Note = "Nota 1",
                    ValueTotal = 100.00m,
                    State = "Active"
                },
                new NoteDto
                {
                    Id = Guid.NewGuid(),
                    Note = "Nota 2",
                    ValueTotal = 250.50m,
                    State = "Active"
                }
            };
            
            var repository = Substitute.For<IInvoiceSimpleQueryRepository>();
            repository.GetAllWithNotesAsync()
                .Returns(expectedNotes);
            
            var handler = new GetNotesHandler(repository);
            
            // Act
            var result = await handler.Handle(new GetNotesQuery(), CancellationToken.None);
            
            // Assert
            var resultArray = Assert.IsAssignableFrom<IEnumerable<NoteDto>>(result);
            var resultList = resultArray.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal("Nota 1", resultList[0].Note);
            Assert.Equal("Nota 2", resultList[1].Note);
        }

        [Fact]
        public async Task Handle_EmptyResult_ReturnsEmptyCollection()
        {
            // Arrange
            var repository = Substitute.For<IInvoiceSimpleQueryRepository>();
            repository.GetAllWithNotesAsync()
                .Returns(Enumerable.Empty<NoteDto>());
            
            var handler = new GetNotesHandler(repository);
            
            // Act
            var result = await handler.Handle(new GetNotesQuery(), CancellationToken.None);
            
            // Assert
            var resultList = Assert.IsAssignableFrom<IEnumerable<NoteDto>>(result);
            Assert.Empty(resultList);
        }

        [Fact]
        public async Task Handle_CancellationToken_PassedToRepository()
        {
            // Arrange
            var repository = Substitute.For<IInvoiceSimpleQueryRepository>();
            repository.GetAllWithNotesAsync()
                .Returns(Enumerable.Empty<NoteDto>());
            
            var handler = new GetNotesHandler(repository);
            var cts = new CancellationTokenSource();
            
            // Act
            await handler.Handle(new GetNotesQuery(), cts.Token);
            
            // Assert
            await repository.Received(1).GetAllWithNotesAsync();
        }

        [Fact]
        public async Task Handle_ReturnsCorrectDataStructure()
        {
            // Arrange
            var testId = Guid.Parse("12345678-1234-1234-1234-123456789012");
            var expectedNotes = new List<NoteDto>
            {
                new NoteDto
                {
                    Id = testId,
                    Note = "Test note",
                    ValueTotal = 999.99m,
                    State = "Active"
                }
            };
            
            var repository = Substitute.For<IInvoiceSimpleQueryRepository>();
            repository.GetAllWithNotesAsync()
                .Returns(expectedNotes);
            
            var handler = new GetNotesHandler(repository);
            
            // Act
            var result = await handler.Handle(new GetNotesQuery(), CancellationToken.None);
            
            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(testId, resultList[0].Id);
            Assert.Equal("Test note", resultList[0].Note);
            Assert.Equal(999.99m, resultList[0].ValueTotal);
            Assert.Equal("Active", resultList[0].State);
        }
    }
}