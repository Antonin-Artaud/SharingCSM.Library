using SharingCsm.Library.Domain.Commons;

namespace SharingCsm.Library.Domain.Books.Exceptions;

public sealed class BookNotAvailableException : DomainException
{
	public Guid BookId { get; }
	public BookNotAvailableException(Guid bookId) : base($"Le livre avec l'ID {bookId} n'est pas disponible pour emprunt.")
	{
		BookId = bookId;
		ErrorCode = "BOOK_NOT_AVAILABLE";
	}
}
