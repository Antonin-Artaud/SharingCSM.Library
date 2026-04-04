using SharingCsm.Library.Domain.Commons;

namespace SharingCsm.Library.Domain.Books.Exceptions;

public sealed class LoanNotFoundOrAlreadyReturnedException : DomainException
{
	public Guid LoanId { get; }
	public LoanNotFoundOrAlreadyReturnedException(Guid loanId) : base($"Le prêt avec l'ID {loanId} n'a pas été trouvé ou a déjà été retourné.")
	{
		LoanId = loanId;
		ErrorCode = "LOAN_NOT_FOUND_OR_ALREADY_RETURNED";
	}
}