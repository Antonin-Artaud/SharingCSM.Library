using SharingCsm.Library.Domain.Commons;

namespace SharingCsm.Library.Domain.Books.ValueObjects;

public sealed record LoanId
{
    public Guid Value { get; }
	
    private LoanId(Guid value) => Value = value;

    public static LoanId Create(Guid id)
    {
        return id == Guid.Empty ? throw new DomainException("LoanId cannot be empty.") : new LoanId(id);
    }
	
    public static implicit operator Guid(LoanId loanId) => loanId.Value;
	
    public override string ToString() => Value.ToString();
}