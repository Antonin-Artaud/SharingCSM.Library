using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.Enums;
using SharingCsm.Library.Domain.Commons;
using System.Linq.Expressions;

namespace SharingCsm.Library.Domain.Books.Specifications;

public sealed class SearchBooksSpecification : Specification<Book>
{
	private readonly string _searchTerm;
	private readonly BookCategory _category;
	private readonly bool _onlyAvailable;

	public SearchBooksSpecification(string searchTerm, BookCategory category, bool onlyAvailable)
	{
		_searchTerm = searchTerm;
		_category = category;
		_onlyAvailable = onlyAvailable;
	}

	public override Expression<Func<Book, bool>> ToExpression() => book =>
			(string.IsNullOrWhiteSpace(_searchTerm) || book.Title.Contains(_searchTerm)) ||
			(book.Category == _category) &&
			(!_onlyAvailable || book.IsAvailable);
}