using Mediator;

namespace SharingCsm.Library.Api.Controllers;

using global::SharingCsm.Library.Application.Books.Handlers;
using global::SharingCsm.Library.Infrastructure.Services.Daos;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using SharingCsm.Library.Application.Books.Dtos;

[ApiController]
[Route("api/[controller]")] 
public class BooksController : ControllerBase
{
	private readonly ISender _sender;

	public BooksController(ISender sender)
	{
		_sender = sender;
	}

	// GET: api/books?searchTerm=Harry&category=Fantasy&onlyAvailable=true&page=1
	[HttpGet]
	public async Task<ActionResult<PagedResult<BookSearchResponse>>> SearchBooks(
		[FromQuery] BookSearchRequest request,
		CancellationToken cancellationToken)
	{
		var query = new SearchBooksQuery(request.SearchTerm, request.Category, request.OnlyAvailable, request.Page, request.PageSize);

		var result = await _sender.Send(query, cancellationToken);

		return Ok(result);
	}
}