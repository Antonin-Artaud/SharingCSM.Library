using Mediator;
using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.Repositories;
using SharingCsm.Library.Domain.Books.ValueObjects;
using SharingCsm.Library.Infrastructure.Services.Catalogs;
using SharingCsm.Library.Infrastructure.UnitOfWorks;

namespace SharingCsm.Library.Application.Catalogs.Handlers;

public sealed record ImportCatalogCommand(Stream Stream) : ICommand;

public class ImportCatalogCommandHandler : ICommandHandler<ImportCatalogCommand>
{
	private readonly IBookRepository _bookRepository;
	private readonly ICatalogImportService _catalogImportService;
	private readonly UnitOfWork _unitOfWork;

	public ImportCatalogCommandHandler(IBookRepository bookRepository, ICatalogImportService catalogImportService, UnitOfWork unitOfWork)
	{
		_bookRepository = bookRepository;
		_catalogImportService = catalogImportService;
		_unitOfWork = unitOfWork;
	}

	public async ValueTask<Unit> Handle(ImportCatalogCommand command, CancellationToken cancellationToken)
	{
		var booksBatch = new List<Book>(1000);

		await foreach (var dao in _catalogImportService.ImportBadAsync(command.Stream, cancellationToken))
		{
			var book = Book.Create(BookId.Create(dao.Isbn), dao.Title, dao.Category);
			booksBatch.Add(book);

			if (booksBatch.Count >= 1000)
			{
				await SaveBatchAsync(booksBatch, cancellationToken);
			}
		}

		if (booksBatch.Count > 0)
		{
			await SaveBatchAsync(booksBatch, cancellationToken);
		}

		return Unit.Value;
	}

	private async Task SaveBatchAsync(List<Book> books, CancellationToken cancellationToken)
	{
		foreach (var book in books)
		{
			await _bookRepository.AddBookAsync(book, cancellationToken);
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);
	}
}
