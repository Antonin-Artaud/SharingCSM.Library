namespace SharingCsm.Library.Domain.Books.Entities;

public class Loan
{
	public Guid Id { get; private set; }
	public Guid BookId { get; private set; }
	public Guid UserId { get; private set; }
	public DateTime DueDate { get; private set; }
	public DateTime? ReturnedDate { get; private set; }

	internal Loan(Guid bookId, Guid userId, DateTime dueDate)
	{
		Id = Guid.NewGuid();
		BookId = bookId;
		UserId = userId;
		DueDate = dueDate;
	}

	public void MarkAsReturned(DateTime returnDate)
	{
		if (ReturnedDate.HasValue)
		{
			throw new InvalidOperationException("This loan has already been returned.");
		}

		ReturnedDate = returnDate;
	}
}