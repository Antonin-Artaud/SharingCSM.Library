using Microsoft.EntityFrameworkCore;
using SharingCsm.Library.Domain.Books.Specifications;
using SharingCsm.Library.Infrastructure.Services.Daos;
using SharingCsm.Library.Infrastructure.Services.Daos.Books;
using SharingCsm.Library.Infrastructure.UnitOfWorks;

namespace SharingCsm.Library.Infrastructure.Services.Books;

internal sealed class BookQueryService : IBookQueryService
{
	private readonly UnitOfWork _unitOfWork;

	public BookQueryService(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

	public async Task<PagedResult<BookSearchDao>> SearchBooksAsync(
		SearchBooksSpecification spec,
		int page,
		int pageSize,
		CancellationToken cancellationToken)
	{
		var query = _unitOfWork.Books
			.AsNoTracking()
			.Where(spec.ToExpression());

		int totalCount = await query.CountAsync(cancellationToken);

		var items = await query
			.OrderBy(b => b.Title)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.Select(b => new BookSearchDao(b.Id.Value, b.Title, b.Category, b.IsAvailable))
			.ToListAsync(cancellationToken);

		return new PagedResult<BookSearchDao>(items, totalCount, page, pageSize);
	}
}