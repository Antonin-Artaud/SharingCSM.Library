using Microsoft.EntityFrameworkCore;
using SharingCsm.Library.Domain.Books.Entities;

namespace SharingCsm.Library.Infrastructure.UnitOfWorks;

public sealed class UnitOfWork : UnitOfWorkBase
{
	public DbSet<Book> Books { get; set; }
	public DbSet<Loan> Loans { get; set; }

	public UnitOfWork(DbContextOptions options) : base(options)
	{
		ChangeTracker.LazyLoadingEnabled = false;
	}
}
