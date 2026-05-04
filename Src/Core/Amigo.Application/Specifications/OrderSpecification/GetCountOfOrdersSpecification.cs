using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetCountOfOrdersSpecification : BaseSpecification<Order, Guid>
    {
        public GetCountOfOrdersSpecification(string userId, GetAllOrdersQuery query)
            : base(
              OrderCommonSpecifciation.BuildCriteriaForUser(query, userId)
            )
        {
        }
    }
}
