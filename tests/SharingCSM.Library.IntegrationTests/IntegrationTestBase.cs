using Mediator;
using Microsoft.EntityFrameworkCore;
using SharingCsm.Library.Domain.Books.Entities;
using SharingCsm.Library.Domain.Books.Enums;
using SharingCsm.Library.Domain.Books.ValueObjects;
using SharingCsm.Library.Infrastructure.UnitOfWorks;

namespace SharingCSM.Library.IntegrationTests;

[Collection("Aspire collection")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly AspireAppFixture _aspireFixture;
    private CustomWebApplicationFactory _factory = null!;
    private IServiceScope _scope = null!;
    
    protected ISender Sender { get; private set; } = null!;
    protected UnitOfWork UnitOfWork { get; private set; } = null!;

    protected IntegrationTestBase(AspireAppFixture aspireFixture)
    {
        _aspireFixture = aspireFixture;
    }

    public Task InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory(_aspireFixture.DbConnectionString);
        
        _scope = _factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        UnitOfWork = _scope.ServiceProvider.GetRequiredService<UnitOfWork>();

        return Task.CompletedTask;
    }
    
    protected async Task<Book> SeedAvailableBookAsync(string title = "Livre par défaut", BookCategory category = BookCategory.SciFi)
    {
        var book = Book.Create(BookId.Create(Guid.NewGuid()), title, category);
        
        UnitOfWork.Books.Add(book);
        await UnitOfWork.SaveChangesAsync();
        
        return book;
    }

    protected async Task<(Book book, Loan loan)> SeedBorrowedBookAsync(Guid userId)
    {
        var book = await SeedAvailableBookAsync();
        
        var loan = book.Borrow(userId, 14);
        
        UnitOfWork.Loans.Add(loan);
        await UnitOfWork.SaveChangesAsync();

        return (book, loan);
    }

    public Task DisposeAsync()
    {
        _scope?.Dispose();
        _factory?.Dispose();
        return Task.CompletedTask;
    }
}

[CollectionDefinition("Aspire collection")]
public class AspireCollection : ICollectionFixture<AspireAppFixture> { }