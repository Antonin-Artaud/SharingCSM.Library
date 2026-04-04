using SharingCsm.Library.Domain.Books.ValueObjects;
using SharingCsm.Library.Domain.Commons;

namespace SharingCsm.Library.Domain.Books.Exceptions;

public sealed class BookNotAvailableException : DomainException
{
	public BookId BookId { get; }
	
	public BookNotAvailableException(BookId bookId) : base($"Le livre avec l'ID {bookId} n'est pas disponible pour emprunt.")
	{
		BookId = bookId;
		ErrorCode = "BOOK_NOT_AVAILABLE";
	}
}
