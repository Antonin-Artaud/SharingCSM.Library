using Microsoft.EntityFrameworkCore;
using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.Repositories;
using SharingCsm.Library.Domain.Books.ValueObjects;
using SharingCsm.Library.Domain.Interfaces;
using SharingCsm.Library.Infrastructure.UnitOfWorks;

namespace SharingCsm.Library.Infrastructure.Repositories.Books;

internal class BookRepository : IBookRepository
{
	private readonly UnitOfWork _unitOfWork;

	public BookRepository(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

	public async Task<Book?> GetByIdAsync(BookId id, CancellationToken cancellationToken)
	{
		return await _unitOfWork.Books
			.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
	}

	public async Task<Book?> GetBySpecificationAsync(ISpecification<Book> specification, CancellationToken cancellationToken)
	{
		return await _unitOfWork.Books
			.Where(specification.ToExpression()) 
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task AddBookAsync(Book book, CancellationToken cancellationToken)
	{
		await _unitOfWork.Books.AddAsync(book, cancellationToken);
	}

	public async Task AddLoanAsync(Loan loan, CancellationToken cancellationToken)
	{
		await _unitOfWork.Loans.AddAsync(loan, cancellationToken);
	}
}
