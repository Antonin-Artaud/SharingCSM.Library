using Microsoft.Extensions.Hosting;
using Projects;

namespace SharingCsm.Library.AppHost.Extensions;

public sealed record InfrastructureModule(
    IResourceBuilder<PostgresDatabaseResource> Database,
    IResourceBuilder<ProjectResource> MigrationsService
);

public static class DataInfrastructureExtensions
{
    public static InfrastructureModule AddInfrastructureModule(this IDistributedApplicationBuilder builder)
    {
        var postgres = builder.AddPostgres(LibraryResourceNames.Postgres)
            .WithPgWeb();
       
        if (!builder.Environment.IsEnvironment("Testing"))
        {
            postgres.WithDataVolume(LibraryResourceNames.PostgresDataVolume);
        }

        var database = postgres.AddDatabase(LibraryResourceNames.Database);

        var migrations = builder.AddProject<SharingCsm_Library_Infrastructure_MigrationsService>(LibraryResourceNames.MigrationsService)
            .WaitFor(database)
            .WithReference(database);

        return new InfrastructureModule(database, migrations);
    }
}