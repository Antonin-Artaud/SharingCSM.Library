using SharingCsm.Library.Application.Books.Dtos;
using SharingCsm.Library.Application.Books.Handlers;
using SharingCsm.Library.Domain.Books.Enums;
using Shouldly;

namespace SharingCSM.Library.IntegrationTests.Handlers.Books;

public class SearchBooksQueryHandlerTests : IntegrationTestBase
{
    public SearchBooksQueryHandlerTests(AspireAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Handle_Should_Return_Paginated_And_Mapped_Results()
    {
        // ---------------------------------------------------------
        // 1. ARRANGE : On prépare un jeu de données conséquent
        // ---------------------------------------------------------
        // On nettoie la base au cas où d'autres tests l'auraient polluée
        UnitOfWork.Books.RemoveRange(UnitOfWork.Books);
        await UnitOfWork.SaveChangesAsync();

        // On insère 5 livres
        await SeedAvailableBookAsync("Tome 1");
        await SeedAvailableBookAsync("Tome 2");
        await SeedAvailableBookAsync("Tome 3");
        await SeedAvailableBookAsync("Tome 4");
        await SeedAvailableBookAsync("Tome 5");

        // On demande la Page 2, avec une taille de 2 éléments
        var query = new SearchBooksQuery(
            SearchTerm: null, 
            Category: null, 
            OnlyAvailable: false, 
            Page: 2, 
            PageSize: 2);

        // ---------------------------------------------------------
        // 2. ACT
        // ---------------------------------------------------------
        var result = await Sender.Send(query);

        // ---------------------------------------------------------
        // 3. ASSERT
        // ---------------------------------------------------------
        result.ShouldNotBeNull();
        
        // Vérification de la pagination
        result.TotalCount.ShouldBe(5, "Il y a 5 livres au total en base");
        result.Page.ShouldBe(2);
        result.PageSize.ShouldBe(2);
        
        // Vérification des éléments retournés (Skip 2, Take 2 = Tomes 3 et 4)
        result.Items.ShouldContain(b => b.Title == "Tome 3");
        result.Items.ShouldContain(b => b.Title == "Tome 4");
        
        // Vérification du mapping DTO
        var firstItem = result.Items.First();
        firstItem.ShouldBeOfType<BookSearchResponse>();
        firstItem.Id.ShouldNotBe(Guid.Empty);
        firstItem.Title.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_Should_Apply_Specification_And_Filter_Results()
    {
        // Arrange
        UnitOfWork.Books.RemoveRange(UnitOfWork.Books);
        await UnitOfWork.SaveChangesAsync();

        await SeedAvailableBookAsync("Harry Potter", BookCategory.Fantasy);
        await SeedAvailableBookAsync("Le Seigneur des Anneaux", BookCategory.Fantasy);
        await SeedAvailableBookAsync("Dune", BookCategory.SciFi); // Ne doit pas matcher
        
        // Emprunt d'un livre (il devient indisponible)
        var userId = Guid.NewGuid();
        var (book, _) = await SeedBorrowedBookAsync(userId);
        // Assurons-nous que le livre emprunté s'appelle aussi "Harry Potter" 
        // (Note: il faudrait ajuster ton SeedBorrowedBookAsync pour accepter le titre si besoin, 
        // ou modifier le titre manuellement ici pour le test)

        // On cherche "Harry" en Fantasy, uniquement disponible
        var query = new SearchBooksQuery(
            SearchTerm: "Harry", 
            Category: BookCategory.Fantasy, 
            OnlyAvailable: true, 
            Page: 1, 
            PageSize: 10);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldContain(b => b.Title.Contains("Harry") && b.IsAvailable);
    }
}