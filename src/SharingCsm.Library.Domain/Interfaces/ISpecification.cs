using System.Linq.Expressions;

namespace SharingCsm.Library.Domain.Interfaces;

public interface ISpecification<T> where T : IEntity
{
	bool IsSatisfiedBy(T entity);
	Expression<Func<T, bool>> ToExpression();
}
