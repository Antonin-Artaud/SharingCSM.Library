using SharingCsm.Library.Infrastructure.Services.Daos.Books;

namespace SharingCsm.Library.Infrastructure.Services.Catalogs;

public interface ICatalogImportService
{
	IAsyncEnumerable<BookImportDao> ImportBadAsync(Stream stream, CancellationToken cancellationToken);
	IAsyncEnumerable<BookImportDao> ImportFastAsync(Stream stream, CancellationToken cancellationToken);
}
