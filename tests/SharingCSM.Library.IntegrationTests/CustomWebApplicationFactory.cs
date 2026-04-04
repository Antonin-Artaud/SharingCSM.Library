using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharingCsm.Library.Api;
using SharingCsm.Library.Infrastructure.UnitOfWorks;

namespace SharingCSM.Library.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public CustomWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:LibraryDb", _connectionString);
        
        builder.ConfigureServices(services =>
        {
            // On supprime le DbContext existant (celui configuré pour le dev)
            services.RemoveAll<DbContextOptions<UnitOfWork>>();

            // On injecte le DbContext branché sur le conteneur géré par Aspire
            services.AddDbContext<UnitOfWork>(options =>
            {
                options.UseNpgsql(_connectionString);
            });
            
            // Note : C'est aussi ici que tu pourrais appliquer les migrations 
            // de base de données automatiques pour tes tests.
        });
    }
}