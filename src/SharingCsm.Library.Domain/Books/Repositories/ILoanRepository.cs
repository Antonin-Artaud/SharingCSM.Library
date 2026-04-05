using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.ValueObjects;

namespace SharingCsm.Library.Domain.Books.Repositories;

public interface ILoanRepository
{
	Task<Loan?> GetByIdAsync(LoanId id, CancellationToken cancellationToken);
}