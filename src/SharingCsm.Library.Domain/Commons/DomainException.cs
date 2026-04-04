namespace SharingCsm.Library.Domain.Commons;

public class DomainException : Exception
{
	public string ErrorCode { get; protected set; } = "DOMAIN_ERROR";

	public DomainException(string message) : base(message)
	{
	}
}
