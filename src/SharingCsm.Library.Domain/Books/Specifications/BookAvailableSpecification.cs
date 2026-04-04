using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Commons;
using System.Linq.Expressions;
using SharingCsm.Library.Domain.Books.ValueObjects;

namespace SharingCsm.Library.Domain.Books.Specifications;

public class BookAvailableSpecification : Specification<Book>
{
	private readonly BookId _bookId;

	public BookAvailableSpecification(BookId bookId) => _bookId = bookId;

	public override Expression<Func<Book, bool>> ToExpression() => book => book.Id == _bookId && book.IsAvailable;
}
