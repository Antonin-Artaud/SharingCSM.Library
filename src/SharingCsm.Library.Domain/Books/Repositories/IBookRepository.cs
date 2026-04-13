using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.ValueObjects;
using SharingCsm.Library.Domain.Interfaces;

namespace SharingCsm.Library.Domain.Books.Repositories;

public interface IBookRepository
{
	Task<Book?> GetByIdAsync(BookId id, CancellationToken cancellationToken);
	Task<Book?> GetBySpecificationAsync(ISpecification<Book> specification, CancellationToken cancellationToken);
	Task AddBookAsync(Book book, CancellationToken cancellationToken);
	Task AddLoanAsync(Loan loan, CancellationToken cancellationToken);
}
