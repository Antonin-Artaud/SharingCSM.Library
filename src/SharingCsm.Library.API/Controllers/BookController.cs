using Mediator;
using Microsoft.AspNetCore.Mvc;
using SharingCsm.Library.Application.Books.Dtos;
using SharingCsm.Library.Application.Books.Handlers;
using SharingCsm.Library.Domain.Books.Enums;
using SharingCsm.Library.Infrastructure.Services.Daos;

namespace SharingCsm.Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class BooksController : ControllerBase
{
	private readonly ISender _sender;

	public BooksController(ISender sender) => _sender = sender;

	[HttpGet]
	public async Task<ActionResult<PagedResult<BookSearchResponse>>> SearchBooks(
		[FromQuery] BookSearchRequest request,
		CancellationToken cancellationToken)
	{
		var query = new SearchBooksQuery(
			request.SearchTerm ?? string.Empty, 
			request.Category ?? BookCategory.Unknown, 
			request.OnlyAvailable,
			request.Page,
			request.PageSize);

		var result = await _sender.Send(query, cancellationToken);

		return Ok(result);
	}
}