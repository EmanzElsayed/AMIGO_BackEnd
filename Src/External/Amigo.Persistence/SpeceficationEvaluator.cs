using Amigo.Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence
{
    public static class SpeceficationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity, TKey>(IQueryable<TEntity> inputQuery, ISpecifications<TEntity, TKey> specifications)
            where TEntity : BaseEntity<TKey>
        {
            var query = inputQuery;

            if (specifications.Criteria is not null)
            {
                query = query.Where(specifications.Criteria);
            }

            #region Ordering
            if (specifications.OrderBY is not null)
            {
                query = query.OrderBy(specifications.OrderBY);
            }
            if (specifications.OrderBYDescending is not null)
            {
                query = query.OrderByDescending(specifications.OrderBYDescending);
            }
            #endregion

            #region Including
            if (specifications.IncludeExpressions is not null && specifications.IncludeExpressions.Any())
            {
                
                query = specifications.IncludeExpressions.Aggregate(query, (current, includeExpression) => current.Include(includeExpression));
            }

            if (specifications.Includes is not null && specifications.Includes.Any())
            {
                query = specifications.Includes
                    .Aggregate(query, (current, include) => include(current));
            }
            #endregion


            #region Pagination

            if (specifications.IsPaginated)
            {
                query = query.Skip(specifications.Skip).Take(specifications.Take);
            }
            #endregion

            return query;

        }
    }
}
