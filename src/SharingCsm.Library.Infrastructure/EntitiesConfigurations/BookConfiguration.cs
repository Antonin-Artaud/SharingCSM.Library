using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.Enums;
using SharingCsm.Library.Domain.Books.ValueObjects;

namespace SharingCsm.Library.Infrastructure.EntitiesConfigurations;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
	public void Configure(EntityTypeBuilder<Book> builder)
	{
		builder.ToTable("Books");

		builder.HasKey(b => b.Id);

		builder.Property(b => b.Id)
			.HasConversion(
				bookId => bookId.Value,
				guidValue => BookId.Create(guidValue)
			);

		builder.Property(x => x.Title)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(x => x.Category)
			.IsRequired()
			.HasConversion(
				category => category.ToString(),
				stringValue => Enum.Parse<BookCategory>(stringValue)
			);

		builder.Property(x => x.IsAvailable);
	}
}
