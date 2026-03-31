using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Domain.Abstraction
{
    public interface ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public Expression<Func<TEntity, bool>>? Criteria { get; }
        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; }
        public Expression<Func<TEntity, object>> OrderBY { get; }
        public Expression<Func<TEntity, object>> OrderBYDescending { get; }
        public int Skip { get; }
        public int Take { get; }
        public bool IsPaginated { get; }
    }
}
