using Microsoft.EntityFrameworkCore;
using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.Enums;
using SharingCsm.Library.Domain.Books.ValueObjects;
using SharingCsm.Library.Infrastructure.UnitOfWorks;
using System.Collections;
using System.Diagnostics;

namespace SharingCsm.Library.Infrastructure.MigrationsService;

public class Worker : BackgroundService
{
	public const string ActivitySourceName = "Migrations";
	private static readonly ActivitySource activitySource = new(ActivitySourceName);
	private readonly IServiceProvider _serviceProvider;
	private readonly IHostApplicationLifetime _hostApplicationLifetime;

	public Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
	{
		_serviceProvider = serviceProvider;
		_hostApplicationLifetime = hostApplicationLifetime;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var activity = activitySource.StartActivity("Migrating database", ActivityKind.Client);

		try
		{
			using var scope = _serviceProvider.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<UnitOfWork>();

			await RunMigrationAsync(dbContext, stoppingToken);
			await SeedDataAsync(dbContext, stoppingToken);
		}
		catch (Exception ex)
		{
			activity?.AddException(ex);
			throw;
		}

		_hostApplicationLifetime.StopApplication();
	}

	private static async Task RunMigrationAsync(
		UnitOfWork dbContext, CancellationToken cancellationToken)
	{
		var strategy = dbContext.Database.CreateExecutionStrategy();
		await strategy.ExecuteAsync(async () =>
		{
			// Run migration in a transaction to avoid partial migration if it fails.
			await dbContext.Database.MigrateAsync(cancellationToken);
		});
	}

	private static async Task SeedDataAsync(UnitOfWork dbContext, CancellationToken cancellationToken)
	{
		var book = Book.Create(BookId.Create(Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6")), "Harry Potter and the Sorcerer's Stone", BookCategory.Fantasy);

		if (!await dbContext.Books.AnyAsync(b => b.Id == book.Id, cancellationToken))
		{
			dbContext.Books.Add(book);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}