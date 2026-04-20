using Amigo.SharedKernal.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications
{
    
    public abstract class UserBaseSpecification
       : IUserSpecification
    {
        protected UserBaseSpecification(Expression<Func<ApplicationUser, bool>>? criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<ApplicationUser, bool>>? Criteria { get; private set; }

        #region Include
        public List<Expression<Func<ApplicationUser, object>>> IncludeExpressions { get; } = [];


        protected void AddInclude(Expression<Func<ApplicationUser, object>> includeExpressions)
        {
            IncludeExpressions.Add(includeExpressions);
        }

        public List<Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, object>>> Includes { get; }
       = new();

        protected void AddInclude(
            Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        #endregion

        #region Ordering
        public Expression<Func<ApplicationUser, object>> OrderBY { get; private set; }
        public Expression<Func<ApplicationUser, object>> OrderBYDescending { get; private set; }


        protected void AddOrderBY(Expression<Func<ApplicationUser, object>> OrderBYExpressions)
        {
            OrderBY = OrderBYExpressions;
        }
        protected void AddOrderBYDescending(Expression<Func<ApplicationUser, object>> OrderBYDescendingExpressions)
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
