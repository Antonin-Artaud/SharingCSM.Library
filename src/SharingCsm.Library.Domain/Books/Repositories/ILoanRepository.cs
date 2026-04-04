using SharingCsm.Library.Domain.Books.Entities;

namespace SharingCsm.Library.Domain.Books.Repositories;

public interface ILoanRepository
{
	Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}