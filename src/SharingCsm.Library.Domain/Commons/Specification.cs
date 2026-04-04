using SharingCsm.Library.Domain.Interfaces;
using System.Linq.Expressions;

namespace SharingCsm.Library.Domain.Commons;

public abstract class Specification<T> : ISpecification<T> where T : IEntity
{
	public abstract Expression<Func<T, bool>> ToExpression();

	private Func<T, bool>? _compiled;

	public bool IsSatisfiedBy(T entity)
	{
		_compiled ??= ToExpression().Compile();
		return _compiled(entity);
	}
}
