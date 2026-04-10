using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SharingCsm.Library.Domain.Commons;

namespace SharingCsm.Library.Api.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;

	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		_logger.LogError(exception, exception.Message);

		var problemDetails = new ProblemDetails
		{
			Instance = httpContext.Request.Path
		};

		switch (exception)
		{
			case DomainException domainEx:
				problemDetails.Title = domainEx.ErrorCode;
				problemDetails.Status = StatusCodes.Status400BadRequest;
				break;
			default:
				problemDetails.Title = "Internal Server Error";
				problemDetails.Status = StatusCodes.Status500InternalServerError;
				problemDetails.Detail = exception.Message;
				break;
		}

		httpContext.Response.StatusCode = problemDetails.Status.Value;

		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

		return true;
	}
}
