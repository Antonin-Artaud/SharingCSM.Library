using Microsoft.Extensions.DependencyInjection;
using SharingCsm.Library.Infrastructure;
using SharingCsm.Library.Infrastructure.UnitOfWorks;

namespace SharingCsm.Library.Application;

public static class LibraryApplicationModule
{

    public static void AddApplicationModule(this IServiceCollection services)
	{
		services.AddMediator(options =>
		{
			options.Assemblies = [
				typeof(LibraryApplicationModule).Assembly,
				typeof(LibraryInfrastructureModule).Assembly,
			];
			options.ServiceLifetime = ServiceLifetime.Scoped;
			options.PipelineBehaviors = [
				typeof(UnitOfWorkBehavior<,>)
			];
		});
	}
}
