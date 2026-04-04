using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Commons;
using System.Linq.Expressions;

namespace SharingCsm.Library.Domain.Books.Specifications;

public class BookAvailableSpecification : Specification<Book>
{
	public Guid BookId { get; }

	public BookAvailableSpecification(Guid bookId) => BookId = bookId;

	public override Expression<Func<Book, bool>> ToExpression() => book => book.Id == BookId && book.IsAvailable;
}
