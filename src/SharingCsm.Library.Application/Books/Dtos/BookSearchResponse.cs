using Mediator;
using SharingCsm.Library.Domain.Books.Enums;

namespace SharingCsm.Library.Application.Books.Dtos;

public sealed record BookSearchResponse(
	Guid Id,
	string Title,
	BookCategory Category,
	bool IsAvailable);
