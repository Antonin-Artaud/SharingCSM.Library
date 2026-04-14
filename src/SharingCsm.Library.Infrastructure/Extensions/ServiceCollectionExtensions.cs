using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharingCsm.Library.Domain.Books.Repositories;
using SharingCsm.Library.Infrastructure.Repositories.Books;
using SharingCsm.Library.Infrastructure.Services.Books;
using SharingCsm.Library.Infrastructure.Services.Catalogs;
using SharingCsm.Library.Infrastructure.UnitOfWorks;

namespace SharingCsm.Library.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IHostApplicationBuilder applicationBuilder)
	{
		public void AddUnitOfWork()
		{
			var connectionString = applicationBuilder.Configuration.GetConnectionString("librarydatabase") 
			                       ?? throw new InvalidOperationException("Connection string not found.");

			if (!applicationBuilder.Environment.IsDevelopment() && !connectionString.Contains("Ssl Mode", StringComparison.OrdinalIgnoreCase))
			{
				connectionString = connectionString.TrimEnd(';') + ";Ssl Mode=Require;";
			}

			applicationBuilder.AddNpgsqlDbContext<UnitOfWork>("librarydatabase", settings =>
			{
				settings.ConnectionString = connectionString;
			});
		}
	}
	
	extension(IServiceCollection services)
	{
		public void AddServices()
		{
			services.AddScoped<IBookQueryService, BookQueryService>();
			services.AddScoped<ICatalogImportService, CatalogImportService>();
		}

		public void AddRepositories()
		{
			services.AddScoped<IBookRepository, BookRepository>();
			services.AddScoped<ILoanRepository, LoanRepository>();
		}
	}
}
