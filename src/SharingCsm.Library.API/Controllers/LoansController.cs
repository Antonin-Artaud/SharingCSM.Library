using Microsoft.AspNetCore.Mvc;
using Mediator;
using SharingCsm.Library.Application.Books.Handlers;
using SharingCsm.Library.Application.Loans.Dtos;

namespace SharingCsm.Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
	private readonly ISender _sender;

	public LoansController(ISender sender) => _sender = sender;

	[HttpPost]
	public async Task<ActionResult<CreateLoanResponse>> CreateLoan(
		[FromBody] CreateLoanRequest request,
		CancellationToken cancellationToken)
	{
		var command = new BorrowBookCommand(request.BookId, request.UserId);

		var newLoanId = await _sender.Send(command, cancellationToken);

		var response = new CreateLoanResponse(newLoanId);

		return CreatedAtAction(nameof(CreateLoan), new { id = newLoanId }, response);
	}

	[HttpPost("{loanId:guid}/return")]
	public async Task<IActionResult> ReturnBook([FromRoute] Guid loanId, CancellationToken cancellationToken)
	{
		var command = new ReturnBookCommand(loanId);

		await _sender.Send(command, cancellationToken);

		return NoContent();
	}
}