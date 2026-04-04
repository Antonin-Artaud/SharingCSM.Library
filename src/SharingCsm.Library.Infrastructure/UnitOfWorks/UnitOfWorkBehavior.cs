using Mediator;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SharingCsm.Library.Infrastructure.UnitOfWorks;

public sealed class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : ICommand, ICommand<TResponse>
{
	private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger;
	private readonly UnitOfWork _unitOfWork;

	public UnitOfWorkBehavior(ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger, UnitOfWork unitOfWork)
	{
		_logger = logger;
		_unitOfWork = unitOfWork;
	}

	public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
	{
		var commandName = typeof(TRequest).Name;
		
		if (_logger.IsEnabled(LogLevel.Information))
		{
			_logger.LogInformation("Begin handling command {CommandName} with payload {@CommandPayload}", [commandName, message]);
		}

		var stopwatch = Stopwatch.StartNew();

		var response = await next(message, cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		stopwatch.Stop();

		if (_logger.IsEnabled(LogLevel.Information))
		{
			_logger.LogInformation(
			"Command {CommandName} handled and saved successfully in {ElapsedMilliseconds} ms",
			[
				commandName,
				stopwatch.ElapsedMilliseconds
			]);
		}

		return response;
	}
}