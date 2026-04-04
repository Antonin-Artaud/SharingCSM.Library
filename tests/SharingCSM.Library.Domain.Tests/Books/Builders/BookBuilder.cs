using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.Enums;
using SharingCsm.Library.Domain.Books.ValueObjects;

namespace SharingCSM.Library.Domain.Tests.Books.Builders;

public class BookBuilder
{
    private readonly BookId _id = BookId.Create(Guid.NewGuid());
    private string _title = "Default Title";
    private BookCategory _category = BookCategory.Unknown;
    private bool _isBorrowed = false;

    public BookBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public BookBuilder WithCategory(BookCategory category)
    {
        _category = category;
        return this;
    }

    public BookBuilder ThatIsBorrowed()
    {
        _isBorrowed = true;
        return this;
    }

    public Book Build()
    {
        var book = Book.Create(_id, _title, _category);

        if (_isBorrowed)
        {
            book.Borrow(Guid.NewGuid(), 14);
        }

        return book;
    }
}