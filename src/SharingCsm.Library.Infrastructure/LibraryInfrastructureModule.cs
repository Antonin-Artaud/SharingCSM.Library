using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharingCsm.Library.Infrastructure.Extensions;

namespace SharingCsm.Library.Infrastructure;

public static class LibraryInfrastructureModule
{
	public static void AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddUnitOfWork(configuration);
		services.AddServices();
		services.AddRepository();
	}
}
