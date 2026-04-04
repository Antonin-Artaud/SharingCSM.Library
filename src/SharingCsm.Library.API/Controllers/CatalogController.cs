using Mediator;
using Microsoft.AspNetCore.Mvc;
using SharingCsm.Library.Application.Catalogs.Handlers;

namespace SharingCsm.Library.Api.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
	private readonly ISender _sender;

	public CatalogController(ISender sender)
	{
		_sender = sender;
	}

	[HttpPost("import/classic")]
	public async Task<IActionResult> ImportClassic(IFormFile file, CancellationToken cancellationToken)
	{
		if (file is null || file.Length == 0)
		{
			return BadRequest("The file is empty or missing.");
		}

		using var stream = file.OpenReadStream();

		var command = new ImportCatalogCommand(stream);
		await _sender.Send(command, cancellationToken);

		return Ok(new { Message = "Classic import completed." });
	}

	[HttpPost("import/fast")]
	public async Task<IActionResult> ImportFast(IFormFile file, CancellationToken cancellationToken)
	{
		if (file is null || file.Length == 0)
		{
			return BadRequest("The file is empty or missing.");
		}

		using var stream = file.OpenReadStream();

		var command = new ImportCatalogFastCommand(stream);
		await _sender.Send(command, cancellationToken);

		return Ok(new { Message = "Fast import completed with zero allocations." });
	}
}