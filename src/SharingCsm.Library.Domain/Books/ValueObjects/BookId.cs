using SharingCsm.Library.Domain.Commons;

namespace SharingCsm.Library.Domain.Books.ValueObjects;

public sealed record BookId
{
	public Guid Value { get; }

	private BookId(Guid value) => Value = value;

	public static BookId Create(Guid id)
	{
		if (id == Guid.Empty)
		{
			throw new DomainException("BookId cannot be empty.");
		}

		return new BookId(id);
	}

	public static implicit operator Guid(BookId bookId) => bookId.Value;
}
