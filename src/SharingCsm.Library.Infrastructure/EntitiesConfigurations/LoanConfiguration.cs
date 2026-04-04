using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharingCsm.Library.Domain.Books.Entities;

namespace SharingCsm.Library.Infrastructure.EntitiesConfigurations;

internal sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
	public void Configure(EntityTypeBuilder<Loan> builder)
	{
		builder.ToTable("Loans");

		builder.HasKey(x => x.Id);

		builder.Property(x => x.BookId)
			.IsRequired();

		builder.Property(x => x.UserId)
			.IsRequired();

		builder.Property(x => x.DueDate)
			.IsRequired();

		builder.Property(x => x.ReturnedDate);
	}
}