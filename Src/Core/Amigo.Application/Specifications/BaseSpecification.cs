using Amigo.Domain.Abstraction;
using Amigo.SharedKernal.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications
{
    public abstract class UserBaseSpecification<TEntity, TKey>
        : ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        protected UserBaseSpecification(Expression<Func<TEntity, bool>>? criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<TEntity, bool>>? Criteria { get; private set; }

        #region Include
        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = [];


        protected void AddInclude(Expression<Func<TEntity, object>> includeExpressions)
        {
            IncludeExpressions.Add(includeExpressions);
        }

        public List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> Includes { get; }
       = new();

        protected void AddInclude(
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        #endregion

        #region Ordering
        public Expression<Func<TEntity, object>> OrderBY { get; private set; }
        public Expression<Func<TEntity, object>> OrderBYDescending { get; private set; }


        protected void AddOrderBY(Expression<Func<TEntity, object>> OrderBYExpressions)
        {
            OrderBY = OrderBYExpressions;
        }
        protected void AddOrderBYDescending(Expression<Func<TEntity, object>> OrderBYDescendingExpressions)
        {
            OrderBYDescending = OrderBYDescendingExpressions;
        }
        #endregion

        #region Pagination

        public int Skip { get; private set; }
        public int Take { get; private set; }
        public bool IsPaginated { get; set; }

        protected void ApplyPagination(int pageSize, int pageIndex)
        {
            IsPaginated = true;
            Take = pageSize;
            Skip = (pageIndex - 1) * pageSize;
        }

        #endregion
    }
}
