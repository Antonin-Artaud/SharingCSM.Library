using SharingCsm.Library.Domain.Books.ValueObjects;
using SharingCsm.Library.Domain.Commons;

namespace SharingCsm.Library.Domain.Books.Exceptions;

public sealed class LoanNotFoundOrAlreadyReturnedException : DomainException
{
	public LoanNotFoundOrAlreadyReturnedException(LoanId loanId) : base($"Le prêt avec l'ID {loanId} n'a pas été trouvé ou a déjà été retourné.")
	{
		ErrorCode = "LOAN_NOT_FOUND_OR_ALREADY_RETURNED";
	}
}