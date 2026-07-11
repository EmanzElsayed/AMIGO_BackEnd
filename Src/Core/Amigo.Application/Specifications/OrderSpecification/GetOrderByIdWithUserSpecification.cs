using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetOrderByIdWithUserSpecification : BaseSpecification<Order, Guid>
    {
        public GetOrderByIdWithUserSpecification(Guid orderId) 
            : base(o => !o.IsDeleted && o.Id == orderId)
        {
            AddInclude(o => o.User);
        }
    }
}
