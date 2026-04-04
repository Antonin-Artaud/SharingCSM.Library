using SharingCsm.Library.Domain.Books.Enums;

namespace SharingCsm.Library.Infrastructure.Services.Daos.Books;

public sealed record BookSearchDao(
	Guid Id,
	string Title,
	BookCategory Category,
	bool IsAvailable);