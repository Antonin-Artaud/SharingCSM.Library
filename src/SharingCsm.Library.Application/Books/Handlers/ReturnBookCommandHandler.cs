using Mediator;
using SharingCsm.Library.Domain.Books.Exceptions;
using SharingCsm.Library.Domain.Books.Repositories;
using SharingCsm.Library.Domain.Books.ValueObjects;

namespace SharingCsm.Library.Application.Books.Handlers;

public sealed record ReturnBookCommand(Guid LoanId) : ICommand;

public sealed class ReturnBookCommandHandler : ICommandHandler<ReturnBookCommand>
{
	private readonly ILoanRepository _loanRepository;
	private readonly IBookRepository _bookRepository;

	public ReturnBookCommandHandler(
		ILoanRepository loanRepository,
		IBookRepository bookRepository)
	{
		_loanRepository = loanRepository;
		_bookRepository = bookRepository;
	}

	public async ValueTask<Unit> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
	{
		var loanId = LoanId.Create(request.LoanId);
		
		var loan = await _loanRepository.GetByIdAsync(loanId, cancellationToken);

		if (loan is null || loan.ReturnedDate.HasValue)
		{
			throw new LoanNotFoundOrAlreadyReturnedException(loanId);
		}

		var book = await _bookRepository.GetByIdAsync(loan.BookId, cancellationToken)
			?? throw new BookNotFoundException(loan.BookId);

		loan.MarkAsReturned(DateTime.UtcNow); 
		book.Return();                        

		return Unit.Value;
	}
}