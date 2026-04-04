using Mediator;
using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.Exceptions;
using SharingCsm.Library.Domain.Books.Repositories;
using SharingCsm.Library.Domain.Books.Specifications;

namespace SharingCsm.Library.Application.Books.Handlers;

public sealed record BorrowBookCommand(Guid BookId, Guid UserId) : ICommand<Guid>;

public class BorrowBookCommandHandler : ICommandHandler<BorrowBookCommand, Guid>
{
	private readonly IBookRepository _bookRepository;

	public BorrowBookCommandHandler(IBookRepository bookRepository)
	{
		_bookRepository = bookRepository;
	}

	public async ValueTask<Guid> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
	{
		var isAvaibleSpec = new BookAvailableSpecification(request.BookId);

		var book = await _bookRepository.GetBySpecificationAsync(isAvaibleSpec, cancellationToken)
			?? throw new BookNotAvailableException(request.BookId);

		var loan = book.Borrow(request.UserId, 14);

		await _bookRepository.AddLoanAsync(loan, cancellationToken);

		return loan.Id;
	}
}
