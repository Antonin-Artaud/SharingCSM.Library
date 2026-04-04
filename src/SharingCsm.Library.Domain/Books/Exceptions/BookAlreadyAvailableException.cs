using SharingCsm.Library.Domain.Commons;

namespace SharingCsm.Library.Domain.Books.Exceptions;

public sealed class BookAlreadyAvailableException : DomainException
{
	public Guid BookId { get; }
	public BookAlreadyAvailableException(Guid bookId) : base($"Le livre avec l'ID {bookId} est déjà disponible et ne peut pas être retourné.")
	{
		BookId = bookId;
		ErrorCode = "BOOK_ALREADY_AVAILABLE";
	}
}