using SharingCsm.Library.Domain.Commons;

namespace SharingCsm.Library.Domain.Books.Exceptions;

public sealed class BookNotFoundException : DomainException
{
	public Guid BookId { get; }
	public BookNotFoundException(Guid bookId) : base($"Le livre avec l'ID {bookId} n'a pas été trouvé.")
	{
		BookId = bookId;
		ErrorCode = "BOOK_NOT_FOUND";
	}
}
