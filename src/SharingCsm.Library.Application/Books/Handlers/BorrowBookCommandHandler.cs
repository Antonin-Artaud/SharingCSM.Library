using Mediator;
using SharingCsm.Library.Domain.Books.Exceptions;
using SharingCsm.Library.Domain.Books.Repositories;
using SharingCsm.Library.Domain.Books.Specifications;
using SharingCsm.Library.Domain.Books.ValueObjects;

namespace SharingCsm.Library.Application.Books.Handlers;

public sealed record BorrowBookCommand(Guid BookId, Guid UserId) : ICommand<LoanId>;

public class BorrowBookCommandHandler : ICommandHandler<BorrowBookCommand, LoanId>
{
	private readonly IBookRepository _bookRepository;

	public BorrowBookCommandHandler(IBookRepository bookRepository)
	{
		_bookRepository = bookRepository;
	}

	public async ValueTask<LoanId> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
	{
		var bookId = BookId.Create(request.BookId);
		var isAvailableSpec = new BookAvailableSpecification(bookId);

		var book = await _bookRepository.GetBySpecificationAsync(isAvailableSpec, cancellationToken)
			?? throw new BookNotAvailableException(bookId);

		var loan = book.Borrow(request.UserId, 14);

		await _bookRepository.AddLoanAsync(loan, cancellationToken);

		return loan.Id;
	}
}
