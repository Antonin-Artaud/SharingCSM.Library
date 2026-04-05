using Mediator;
using SharingCsm.Library.Application.Books.Dtos;
using SharingCsm.Library.Domain.Books.Enums;
using SharingCsm.Library.Domain.Books.Specifications;
using SharingCsm.Library.Infrastructure.Services.Books;
using SharingCsm.Library.Infrastructure.Services.Daos;

namespace SharingCsm.Library.Application.Books.Handlers;

public sealed record SearchBooksQuery(
	string SearchTerm,
	BookCategory Category,
	bool OnlyAvailable,
	int Page = 1,
	int PageSize = 20) : IQuery<PagedResult<BookSearchResponse>>;

public sealed class SearchBooksQueryHandler : IQueryHandler<SearchBooksQuery, PagedResult<BookSearchResponse>>
{
	private readonly IBookQueryService _bookQueryService;

	public SearchBooksQueryHandler(IBookQueryService bookQueryService) => _bookQueryService = bookQueryService;

	public async ValueTask<PagedResult<BookSearchResponse>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
	{
		var spec = new SearchBooksSpecification(request.SearchTerm, request.Category, request.OnlyAvailable);

		var result = await _bookQueryService.SearchBooksAsync(spec, request.Page, request.PageSize, cancellationToken);

		var items = result.Items.Select(b => new BookSearchResponse(b.Id, b.Title, b.Category, b.IsAvailable)).ToList();

		return new PagedResult<BookSearchResponse>(items, result.TotalCount, request.Page, request.PageSize);
	}
}