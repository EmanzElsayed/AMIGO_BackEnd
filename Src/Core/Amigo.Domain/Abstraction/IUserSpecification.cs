using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Domain.Abstraction
{
    public interface IUserSpecification
    {
        public Expression<Func<ApplicationUser, bool>>? Criteria { get; }
        public List<Expression<Func<ApplicationUser, object>>> IncludeExpressions { get; }

        public List<Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, object>>> Includes { get; }
        public Expression<Func<ApplicationUser, object>> OrderBY { get; }
        public Expression<Func<ApplicationUser, object>> OrderBYDescending { get; }
        public int Skip { get; }
        public int Take { get; }
        public bool IsPaginated { get; }
    }
}
