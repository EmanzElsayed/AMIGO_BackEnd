using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetCountOfOrdersForAdminSpecification : BaseSpecification<Order, Guid>
    {
        public GetCountOfOrdersForAdminSpecification(GetAllAdminOrdersQuery query)
            : base(
              OrderCommonSpecifciation.BuildCriteriaForAdmin(query)
            )
        {
        }
    }
}
