using Microsoft.EntityFrameworkCore;
using SharingCsm.Library.Infrastructure.UnitOfWorks;

namespace SharingCsm.Library.Infrastructure.MigrationsService;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);
		builder.AddServiceDefaults();
		builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

		builder.Services.AddHostedService<Worker>();
		
		builder.AddNpgsqlDbContext<UnitOfWork>("library-database");

		var host = builder.Build();

		await host.RunAsync();
	}
}
