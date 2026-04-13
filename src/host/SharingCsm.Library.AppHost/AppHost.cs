using Projects;
using SharingCsm.Library.AppHost.Extensions;

namespace SharingCsm.Library.AppHost;

public abstract class Program
{
	private static async Task Main(string[] args)
	{
		var builder = DistributedApplication.CreateBuilder(args);

		var infrastructureModule = builder.AddInfrastructureModule();

		var api = builder.AddProject<SharingCsm_Library_Api>(LibraryResourceNames.Api)
			.WithReference(infrastructureModule.Database)
			.WaitFor(infrastructureModule.Database)
			.WaitForCompletion(infrastructureModule.MigrationsService)
			.WithUrl("/swagger", "Library API Swagger UI")
			.WithExternalHttpEndpoints();

		builder.AddJavaScriptApp("frontend", "../../../library-ui", "start")
			.WithReference(api)
			.WaitFor(api)
			.WithHttpEndpoint(env: "PORT")
			.WithExternalHttpEndpoints()
			.PublishAsDockerFile();

		await builder.Build().RunAsync();
	}
}