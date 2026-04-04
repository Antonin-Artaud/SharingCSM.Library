using SharingCsm.Library.Application.Books.Handlers;
using SharingCsm.Library.Domain.Books.Exceptions;
using Shouldly;

namespace SharingCSM.Library.IntegrationTests.Handlers.Books;

public class BorrowBookCommandHandlerTests : IntegrationTestBase
{
    public BorrowBookCommandHandlerTests(AspireAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Handle_Should_Create_Loan_And_Make_Book_Unavailable()
    {
        // ---------------------------------------------------------
        // 1. ARRANGE : On prépare un livre disponible
        // ---------------------------------------------------------
        var book = await SeedAvailableBookAsync("Le guide du voyageur galactique");
        var userId = Guid.NewGuid();

        var command = new BorrowBookCommand(book.Id.Value, userId);

        // ---------------------------------------------------------
        // 2. ACT : On exécute la commande d'emprunt
        // ---------------------------------------------------------
        var loanId = await Sender.Send(command);

        // ---------------------------------------------------------
        // 3. ASSERT : On vérifie les mutations en base de données
        // ---------------------------------------------------------
        UnitOfWork.ChangeTracker.Clear(); // On vide le cache d'EF Core

        var updatedBook = await UnitOfWork.Books.FindAsync(book.Id);
        var createdLoan = await UnitOfWork.Loans.FindAsync(loanId);

        // Vérification du livre
        updatedBook.ShouldNotBeNull();
        updatedBook!.IsAvailable.ShouldBeFalse("Le livre vient d'être emprunté, il ne doit plus être disponible");

        // Vérification de l'emprunt
        createdLoan.ShouldNotBeNull("L'emprunt doit être sauvegardé en base de données");
        createdLoan!.UserId.ShouldBe(userId);
        createdLoan.BookId.ShouldBe(book.Id.Value);
        createdLoan.ReturnedDate.ShouldBeNull("Un nouvel emprunt ne peut pas être déjà retourné");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Book_Does_Not_Exist()
    {
        // Arrange : Un ID de livre totalement inventé
        var unknownBookId = Guid.NewGuid();
        var command = new BorrowBookCommand(unknownBookId, Guid.NewGuid());

        // Act
        Func<Task> action = async () => await Sender.Send(command);

        // Assert
        // Notre repository renvoie null car l'ID n'existe pas, 
        // le Handler doit lever l'exception métier appropriée.
        (await action.ShouldThrowAsync<BookNotAvailableException>())
            .Message.ShouldContain(unknownBookId.ToString());
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Book_Is_Already_Borrowed()
    {
        // Arrange : On prépare un livre qui est DÉJÀ emprunté par quelqu'un d'autre
        var firstUserId = Guid.NewGuid();
        var (book, _) = await SeedBorrowedBookAsync(firstUserId);

        var secondUserId = Guid.NewGuid();
        var command = new BorrowBookCommand(book.Id.Value, secondUserId);

        // Act
        Func<Task> action = async () => await Sender.Send(command);

        // Assert
        // La spécification BookAvailableSpecification va filtrer ce livre.
        // Le repository renverra null, simulant un livre introuvable/indisponible.
        (await action.ShouldThrowAsync<BookNotAvailableException>())
            .Message.ShouldContain(book.Id.ToString());
    }
}