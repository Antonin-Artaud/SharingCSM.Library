using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.Enums;
using SharingCsm.Library.Domain.Books.Specifications;
using SharingCSM.Library.Domain.Tests.Books.Builders;
using Shouldly;

namespace SharingCSM.Library.Domain.Tests.Books.Specifications;

public class SearchBooksSpecificationTests
{
    private readonly List<Book> _books =
    [
        new BookBuilder()
            .WithTitle("Harry Potter à l'école des sorciers")
            .WithCategory(BookCategory.Fantasy)
            .Build(),
        new BookBuilder()
            .WithTitle("Harry Potter et la chambre des Secrets")
            .WithCategory(BookCategory.Fantasy)
            .Build(),
        new BookBuilder()
            .WithTitle("Le Seigneur des Anneaux")
            .WithCategory(BookCategory.Fantasy)
            .ThatIsBorrowed() // this one will be unavailable 
            .Build(),
        new BookBuilder()
            .WithTitle("Dune")
            .WithCategory(BookCategory.SciFi)
            .Build()
    ];

    [Fact]
    public void Should_Return_Any_Books_That_Match_SearchTerm()
    {
        // Arrange
        var spec = new SearchBooksSpecification("Harry", null);
        
        // Action
        var  result = _books.AsQueryable().Where(spec.ToExpression()).ToList();
        
        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(_books.Count(b => b.Title.Contains("Harry")));
        result.ShouldAllBe(b => b.Title.Contains("Harry Potter", StringComparison.OrdinalIgnoreCase));
    }
    
    [Fact]
    public void Should_Return_Empty_When_No_Book_Matches()
    {
        // Arrange
        var spec = new SearchBooksSpecification("Star Wars", null, false);
    
        // Action
        var result = _books.AsQueryable().Where(spec.ToExpression()).ToList();
    
        // Assert
        result.ShouldBeEmpty();
    }
    
    [Fact]
    public void Should_Return_All_Available_Books_When_SearchTerm_Is_Only_Spaces()
    {
        // Arrange
        var spec = new SearchBooksSpecification("   ", null, true);
    
        // Action
        var result = _books.AsQueryable().Where(spec.ToExpression()).ToList();
    
        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(_books.Count(b => b.IsAvailable));
    }
    
    [Fact]
    public void Should_Return_Only_Available_Books()
    {
        // Arrange
        var spec = new SearchBooksSpecification(null, null);

        // Act
        var result = _books.AsQueryable().Where(spec.ToExpression()).ToList();

        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(3);
        result.ShouldAllBe(b => b.IsAvailable == true);
    }
    
    [Fact]
    public void Should_Return_All_Books_When_OnlyAvailable_Is_False()
    {
        // Arrange
        var spec = new SearchBooksSpecification(null, null, false);
        
        // Action
        var result = _books.AsQueryable().Where(spec.ToExpression()).ToList();
        
        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(_books.Count);
    }

    [Theory]
    [InlineData(BookCategory.Fantasy, 3)]
    [InlineData(BookCategory.SciFi, 1)]
    public void Should_Return_All_Books_That_Match_Category(BookCategory category, int expectedCount)
    {
        // Arrange
        var spec = new SearchBooksSpecification(null, category, false);
        
        // Action
        var result = _books.AsQueryable().Where(spec.ToExpression()).ToList();
        
        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(expectedCount);
        result.ShouldAllBe(b => b.Category == category);
    }
    
    [Fact]
    public void Should_Return_Empty_When_Match_Search_And_Category_But_Is_Unavailable()
    {
        // Arrange : On cherche le Seigneur des anneaux, mais on ne veut que les "Disponibles"
        var spec = new SearchBooksSpecification("Anneaux", BookCategory.Fantasy, onlyAvailable: true);
    
        // Action
        var result = _books.AsQueryable().Where(spec.ToExpression()).ToList();
    
        // Assert
        result.ShouldBeEmpty();
    }
}