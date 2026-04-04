namespace SharingCsm.Library.Application.Loans.Dtos;

public record CreateLoanRequest
{
	public Guid BookId { get; set; }
	public Guid UserId { get; set; }
}