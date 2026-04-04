using SharingCsm.Library.Domain.Books.Enums;
using SharingCsm.Library.Domain.Books.Exceptions;
using SharingCsm.Library.Domain.Books.ValueObjects;
using SharingCsm.Library.Domain.Interfaces;

namespace SharingCsm.Library.Domain.Books.Entities;

public class Book : IEntity
{
	public BookId Id { get; init; }
	public string Title { get; private set; }
	public bool IsAvailable { get; private set; }
	public BookCategory Category { get; private set; }

	private Book() 
	{
		Id = null!;
		Title = string.Empty;
		IsAvailable = false;
		Category = BookCategory.Unknown;
	}

	public static Book Create(BookId id, string title, BookCategory category) => new Book
	{
		Id = id,
		Title = title,
		IsAvailable = true,
		Category = category,
	};

	public Loan Borrow(Guid userId, int maxBorrowDays)
	{
		if (!IsAvailable)
		{
			throw new BookNotAvailableException(Id);
		}

		IsAvailable = false;

		return new Loan(Id, userId, DateTime.UtcNow.AddDays(maxBorrowDays));
	}

	public void Return()
	{
		if (IsAvailable)
		{
			throw new BookAlreadyAvailableException(Id);
		}

		IsAvailable = true;
	}
}
