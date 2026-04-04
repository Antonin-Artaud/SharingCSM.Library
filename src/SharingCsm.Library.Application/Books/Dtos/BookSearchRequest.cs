using SharingCsm.Library.Domain.Books.Enums;

namespace SharingCsm.Library.Application.Books.Dtos;

public sealed record BookSearchRequest(
	string SearchTerm,
	BookCategory Category,
	bool OnlyAvailable,
	int Page = 1,
	int PageSize = 20);
