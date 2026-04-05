using Aspire.Hosting;
using Aspire.Hosting.Testing;
using SharingCsm.Library.AppHost;

namespace SharingCSM.Library.IntegrationTests;

public class AspireAppFixture : IAsyncLifetime
{
    public DistributedApplication App { get; private set; } = null!;
    
    // On va stocker la chaîne de connexion générée par Aspire ici
    public string DbConnectionString { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // 1. On charge l'orchestrateur Aspire (ton projet AppHost)
        var appHostType = typeof(Projects.SharingCsm_Library_AppHost);
        
        string[] args = ["--environment", "Testing"];
        var builder = await DistributedApplicationTestingBuilder.CreateAsync(appHostType, args);

        // 2. On construit et on démarre l'infrastructure (les conteneurs Docker via Aspire)
        App = await builder.BuildAsync();
        await App.StartAsync();

        // 3. On récupère la chaîne de connexion de ta base de données !
        // ⚠️ Remplace "library-db" par le nom exact que tu as donné à ta ressource dans ton AppHost
        // ex: builder.AddPostgres("library-db")
        var connectionString = await App.GetConnectionStringAsync(LibraryResourceNames.Database);
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Impossible de récupérer la chaîne de connexion depuis Aspire.");
        }

        DbConnectionString = connectionString;
    }

    public async Task DisposeAsync()
    {
        if (App is null) return;
        await App.DisposeAsync();
    }
}