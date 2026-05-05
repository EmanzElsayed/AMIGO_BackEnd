using Amigo.Application.Helpers;
using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetAllOrdersSpecification : BaseSpecification<Order, Guid>
    {
        public GetAllOrdersSpecification(string userId , GetAllOrdersQuery query)
            : base(
              OrderCommonSpecifciation.BuildCriteriaForUser(query,userId)
            )
        {
            AddInclude(o => o.Include(i => i.OrderItems).
            ThenInclude(i => i.OrderedPrice));
            AddInclude(o => o.Include(i => i.OrderItems).ThenInclude(i => i.Booking ));

            AddInclude(o => o.Payments);
        }
    }
}
