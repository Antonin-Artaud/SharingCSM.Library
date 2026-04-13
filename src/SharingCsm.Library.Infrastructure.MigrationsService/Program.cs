using SharingCsm.Library.Infrastructure;

namespace SharingCsm.Library.Infrastructure.MigrationsService;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);
		builder.AddServiceDefaults();
		builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

		builder.Services.AddHostedService<Worker>();

		builder.AddInfrastructureModule();

		var host = builder.Build();

		await host.RunAsync();
	}
}
