using SharingCsm.Library.Application.Books.Handlers;
using SharingCsm.Library.Domain.Books.Exceptions;
using Shouldly;

namespace SharingCSM.Library.IntegrationTests.Handlers.Books;

public class ReturnBookCommandHandlerTests : IntegrationTestBase
{
    public ReturnBookCommandHandlerTests(AspireAppFixture aspireFixture) : base(aspireFixture)
    {
    }

    [Fact]
    public async Task Handle_Should_Mark_Book_As_Available_And_Close_Loan()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var (book, loan) = await SeedBorrowedBookAsync(userId);

        var command = new ReturnBookCommand(loan.Id);

        // Action
        await Sender.Send(command);

        // Assert
        UnitOfWork.ChangeTracker.Clear();

        var updatedBook = await UnitOfWork.Books.FindAsync(book.Id);
        var updatedLoan = await UnitOfWork.Loans.FindAsync(loan.Id);

        updatedBook!.IsAvailable.ShouldBeTrue();
        updatedLoan!.ReturnedDate.ShouldNotBeNull();
    }
    
    [Fact]
    public async Task Handle_Should_Throw_When_Loan_Does_Not_Exist()
    {
        // Arrange 
        var unknownLoanId = Guid.NewGuid();
        var command = new ReturnBookCommand(unknownLoanId);

        // Act
        Func<Task> action = async () => await Sender.Send(command);

        // Assert
        var contains = (await action.ShouldThrowAsync<LoanNotFoundOrAlreadyReturnedException>()).Message.Contains(unknownLoanId.ToString());
        contains.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Loan_Is_Already_Returned()
    {
        // Arrange 
        var userId = Guid.NewGuid();
        var (book, loan) = await SeedBorrowedBookAsync(userId);
        
        await Sender.Send(new ReturnBookCommand(loan.Id));

        var command = new ReturnBookCommand(loan.Id);

        // Act
        Func<Task> action = async () => await Sender.Send(command);

        // Assert
        await action.ShouldThrowAsync<LoanNotFoundOrAlreadyReturnedException>();
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Book_Does_Not_Exist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var (book, loan) = await SeedBorrowedBookAsync(userId);

        UnitOfWork.Books.Remove(book);
        await UnitOfWork.SaveChangesAsync();

        var command = new ReturnBookCommand(loan.Id);

        // Act
        Func<Task> action = async () => await Sender.Send(command);

        // Assert
        (await action.ShouldThrowAsync<BookNotFoundException>())
            .Message.ShouldContain(book.Id.ToString());
    }
}