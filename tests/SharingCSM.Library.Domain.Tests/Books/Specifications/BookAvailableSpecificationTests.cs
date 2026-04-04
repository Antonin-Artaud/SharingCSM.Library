using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.Specifications;
using SharingCsm.Library.Domain.Books.ValueObjects;
using SharingCSM.Library.Domain.Tests.Books.Builders;
using Shouldly;
using Xunit;

namespace SharingCSM.Library.Domain.Tests.Books.Specifications;

public class BookAvailableSpecificationTests
{
    private readonly Book _availableBook;
    private readonly Book _unavailableBook;
    private readonly List<Book> _books;

    public BookAvailableSpecificationTests()
    {
        _availableBook = new BookBuilder()
            .WithTitle("Livre Disponible")
            .Build();

        _unavailableBook = new BookBuilder()
            .WithTitle("Livre Emprunté")
            .ThatIsBorrowed()
            .Build();

        _books = [_availableBook, _unavailableBook];
    }

    [Fact]
    public void Should_Return_Book_When_Id_Matches_And_Is_Available()
    {
        // Arrange
        var spec = new BookAvailableSpecification(_availableBook.Id);

        // Act
        var result = _books.AsQueryable().Where(spec.ToExpression()).ToList();

        // Assert
        result.ShouldHaveSingleItem();
        result.First().Id.ShouldBe(_availableBook.Id);
    }

    [Fact]
    public void Should_Return_Empty_When_Id_Matches_But_Is_Unavailable()
    {
        // Arrange
        var spec = new BookAvailableSpecification(_unavailableBook.Id);

        // Act
        var result = _books.AsQueryable().Where(spec.ToExpression()).ToList();

        // Assert
        result.ShouldBeEmpty(); 
    }

    [Fact]
    public void Should_Return_Empty_When_Id_Does_Not_Exist()
    {
        // Arrange
        var unknownId = BookId.Create(Guid.NewGuid());
        var spec = new BookAvailableSpecification(unknownId);

        // Act
        var result = _books.AsQueryable().Where(spec.ToExpression()).ToList();

        // Assert
        result.ShouldBeEmpty();
    }
}