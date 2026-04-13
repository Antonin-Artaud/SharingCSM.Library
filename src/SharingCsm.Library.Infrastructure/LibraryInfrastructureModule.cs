using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharingCsm.Library.Infrastructure.Extensions;

namespace SharingCsm.Library.Infrastructure;

public static class LibraryInfrastructureModule
{
	public static void AddInfrastructureModule(this IHostApplicationBuilder applicationBuilder)
	{
		applicationBuilder.AddUnitOfWork();

		applicationBuilder.Services.AddServices();
		applicationBuilder.Services.AddRepository();
	}
}
