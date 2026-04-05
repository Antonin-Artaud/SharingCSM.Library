using Microsoft.EntityFrameworkCore;
using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.Repositories;
using SharingCsm.Library.Domain.Books.ValueObjects;
using SharingCsm.Library.Infrastructure.UnitOfWorks;

namespace SharingCsm.Library.Infrastructure.Repositories.Books;

internal class LoanRepository : ILoanRepository
{
	private readonly UnitOfWork _unitOfWork;
	public LoanRepository(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

	public async Task<Loan?> GetByIdAsync(LoanId id, CancellationToken cancellationToken)
	{
		return await _unitOfWork.Loans
			.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
	}
}