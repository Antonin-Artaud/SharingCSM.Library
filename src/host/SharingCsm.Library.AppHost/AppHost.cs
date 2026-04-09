using Aspire.Hosting;
using Projects;
using SharingCsm.Library.AppHost.Extensions;

namespace SharingCsm.Library.AppHost;

public abstract class Program
{
	private static async Task Main(string[] args)
	{
		var builder = DistributedApplication.CreateBuilder(args);

		var infrastructureModule = builder.AddInfrastructureModule();

		builder.AddProject<SharingCsm_Library_Api>(LibraryResourceNames.Api)
			.WithReference(infrastructureModule.Database)
			.WaitFor(infrastructureModule.Database)
			.WaitForCompletion(infrastructureModule.MigrationsService)
			.WithExternalHttpEndpoints()
			.WithUrl("/swagger", "Library API Swagger UI");

		await builder.Build().RunAsync();
	}
}