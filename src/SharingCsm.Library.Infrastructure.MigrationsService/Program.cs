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
		
		var connectionString = builder.Configuration.GetConnectionString("librarydatabase");
		
		if (!builder.Environment.IsDevelopment() && !string.IsNullOrEmpty(connectionString) && !connectionString.Contains("Ssl Mode", StringComparison.OrdinalIgnoreCase))
		{
			connectionString = connectionString.TrimEnd(';') + ";Ssl Mode=Require;";
		}
		
		builder.AddNpgsqlDbContext<UnitOfWork>("librarydatabase", settings =>
		{
			if (!string.IsNullOrEmpty(connectionString))
			{
				settings.ConnectionString = connectionString; 
			}
		});

		var host = builder.Build();

		await host.RunAsync();
	}
}
