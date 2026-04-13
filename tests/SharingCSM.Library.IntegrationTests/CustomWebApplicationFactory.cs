using Microsoft.AspNetCore.Mvc.Testing;
using SharingCsm.Library.AppHost;
using Program = SharingCsm.Library.Api.Program;

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
        builder.UseSetting($"ConnectionStrings:{LibraryResourceNames.Database}", _connectionString);
    }
}