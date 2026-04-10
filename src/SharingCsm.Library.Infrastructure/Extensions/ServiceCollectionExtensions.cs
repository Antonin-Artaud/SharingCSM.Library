using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharingCsm.Library.Domain.Books.Repositories;
using SharingCsm.Library.Infrastructure.Repositories.Books;
using SharingCsm.Library.Infrastructure.Services.Books;
using SharingCsm.Library.Infrastructure.Services.Catalogs;
using SharingCsm.Library.Infrastructure.UnitOfWorks;

namespace SharingCsm.Library.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection applicationBuilder)
	{
		public void AddUnitOfWork(IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("library-database")
								   ?? throw new InvalidOperationException();

			applicationBuilder.AddEntityFrameworkNpgsql()
				.AddDbContext<UnitOfWork>(options =>
					options.UseNpgsql(connectionString));
		}

		public void AddServices()
		{
			applicationBuilder.AddScoped<IBookQueryService, BookQueryService>();
			applicationBuilder.AddScoped<ICatalogImportService, CatalogImportService>();
		}

		public void AddRepository()
		{
			applicationBuilder.AddScoped<IBookRepository, BookRepository>();
			applicationBuilder.AddScoped<ILoanRepository, LoanRepository>();
		}
	}
}
