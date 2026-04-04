using SharingCsm.Library.Domain.Books.Specifications;
using SharingCsm.Library.Infrastructure.Services.Daos;
using SharingCsm.Library.Infrastructure.Services.Daos.Books;

namespace SharingCsm.Library.Infrastructure.Services.Books;

public interface IBookQueryService
{
	Task<PagedResult<BookSearchDao>> SearchBooksAsync(
		SearchBooksSpecification spec,
		int page,
		int pageSize,
		CancellationToken cancellationToken);
}
