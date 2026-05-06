using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetAllOrdersForAdminSpecification : BaseSpecification<Order, Guid>
    {
        public GetAllOrdersForAdminSpecification(GetAllAdminOrdersQuery query)
            : base(OrderCommonSpecifciation.BuildCriteriaForAdmin(query))
        {

            AddInclude(o => o.Include(i => i.OrderItems).
            ThenInclude(i => i.OrderedPrice));
            AddInclude(o => o.Include(i => i.OrderItems).ThenInclude(i => i.Booking));
            AddInclude(o => o.User);
            AddInclude(o => o.Payments);
            ApplyPagination(query.PageSize, query.PageNumber);
        }
    }
}
