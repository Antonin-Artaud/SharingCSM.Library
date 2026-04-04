using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SharingCsm.Library.Infrastructure.UnitOfWorks;

public sealed class UnitOfWorkFactory : IDesignTimeDbContextFactory<UnitOfWork>
{
	public UnitOfWork CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<UnitOfWork>();

		optionsBuilder.UseNpgsql(string.Empty);

		return new UnitOfWork(optionsBuilder.Options);
	}
}