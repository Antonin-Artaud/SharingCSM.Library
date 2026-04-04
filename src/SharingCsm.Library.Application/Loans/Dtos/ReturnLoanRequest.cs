namespace SharingCsm.Library.Application.Loans.Dtos;

public record ReturnLoanRequest
{
	public Guid LoanId { get; init; }
}
