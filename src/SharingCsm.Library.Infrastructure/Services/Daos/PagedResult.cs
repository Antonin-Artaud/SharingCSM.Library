namespace SharingCsm.Library.Infrastructure.Services.Daos;

public sealed record PagedResult<T>(
	IEnumerable<T> Items,
	int TotalCount,
	int Page,
	int PageSize);