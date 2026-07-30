using FooBar.Domain.Exceptions;
using FooBar.Domain.Invoices.Model.Entity;
using FooBar.Domain.Tests.Invoices.Model.Entity;
using NSubstitute;

namespace FooBar.Domain.Tests.Invoices.Model;

public class InvoiceNoteTests
{
    [Fact]
    public void SetNote_Success()
    {
        // Arrange
        var invoice = new InvoiceDataBuilder().Build();
        
        // Act
        invoice.SetNote("Entrega urgente");
        
        // Assert
        Assert.Equal("Entrega urgente", invoice.Note);
    }

    [Fact]
    public void SetNote_WithSpaces_TrimsSpaces()
    {
        // Arrange
        var invoice = new InvoiceDataBuilder().Build();
        
        // Act
        invoice.SetNote("  Entrega urgente  ");
        
        // Assert
        Assert.Equal("Entrega urgente", invoice.Note);
    }

    [Fact]
    public void SetNote_NullOrEmpty_RequiredException()
    {
        // Arrange
        var invoice = new InvoiceDataBuilder().Build();
        
        // Act & Assert
        var exception = Assert.Throws<RequiredException>(() =>
            invoice.SetNote(""));
        
        Assert.Equal("the note should not be null or empty.", exception.Message);
    }

    [Fact]
    public void SetNote_ExceedsMaxLength_RequiredException()
    {
        // Arrange
        var invoice = new InvoiceDataBuilder().Build();
        var longNote = new string('a', 501);
        
        // Act & Assert
        var exception = Assert.Throws<RequiredException>(() =>
            invoice.SetNote(longNote));
        
        Assert.Equal("the note should be between 1 and 500 characters.", exception.Message);
    }

    [Fact]
    public void SetNote_CanceledInvoice_CoreBusinessException()
    {
        // Arrange
        var invoice = new InvoiceDataBuilder()
            .WithState(InvoiceState.Canceled)
            .Build();
        
        // Act & Assert
        var exception = Assert.Throws<CoreBusinessException>(() =>
            invoice.SetNote("No debería poder agregar nota"));
        
        Assert.Equal("cannot add a note to a canceled invoice.", exception.Message);
    }

    [Fact]
    public void SetNote_ActiveInvoice_Success()
    {
        // Arrange
        var invoice = new InvoiceDataBuilder()
            .WithState(InvoiceState.Active)
            .Build();
        
        // Act
        invoice.SetNote("Nota de prueba");
        
        // Assert
        Assert.Equal("Nota de prueba", invoice.Note);
        Assert.Equal(InvoiceState.Active, invoice.State);
    }

    [Fact]
    public void Note_DefaultIsNull()
    {
        // Arrange & Act
        var invoice = new InvoiceDataBuilder().Build();
        
        // Assert
        Assert.Null(invoice.Note);
    }

    [Fact]
    public void SetNote_ExactMaxLength_Success()
    {
        // Arrange
        var invoice = new InvoiceDataBuilder().Build();
        var exactNote = new string('a', 500);
        
        // Act
        invoice.SetNote(exactNote);
        
        // Assert
        Assert.Equal(exactNote, invoice.Note);
        Assert.Equal(500, invoice.Note!.Length);
    }
}