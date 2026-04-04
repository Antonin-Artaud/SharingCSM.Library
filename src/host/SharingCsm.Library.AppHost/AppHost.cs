using Projects;

namespace SharingCsm.Library.AppHost;

public static class Program
{
	private static async Task Main(string[] args)
	{
		var builder = DistributedApplication.CreateBuilder(args);

		var postgres = builder.AddPostgres("postgres")
			.WithDataVolume("library-db-data")
			.WithPgWeb();

		var postgresDb = postgres.AddDatabase("LibraryDb");

		var migrations = builder.AddProject<SharingCsm_Library_Infrastructure_MigrationsService>("library-migrations")
			.WaitFor(postgresDb)
			.WithReference(postgresDb);

		builder.AddProject<SharingCsm_Library_Api>("library-api")
			.WithReference(postgresDb)
			.WaitFor(postgresDb)
			.WaitForCompletion(migrations)
			.WithUrl("/swagger", "Library API Swagger UI");

		await builder.Build().RunAsync();
	}
}