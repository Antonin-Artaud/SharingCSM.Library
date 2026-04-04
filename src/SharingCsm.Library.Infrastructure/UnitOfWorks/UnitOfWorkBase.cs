using Microsoft.EntityFrameworkCore;

namespace SharingCsm.Library.Infrastructure.UnitOfWorks;

public abstract class UnitOfWorkBase : DbContext
{
	protected UnitOfWorkBase(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
	}
}