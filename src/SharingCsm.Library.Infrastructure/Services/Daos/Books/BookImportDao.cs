using SharingCsm.Library.Domain.Books.Enums;

namespace SharingCsm.Library.Infrastructure.Services.Daos.Books;

public record BookImportDao(Guid Isbn, string Title, BookCategory Category);