using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetNotDeletedOrderByIdSpecification : BaseSpecification<Order, Guid>
    {
        public GetNotDeletedOrderByIdSpecification(Guid orderId) 
            : base(o => o.Id == orderId && !o.IsDeleted)
        {
            AddInclude(t => t
               .Include(t => t.OrderItems)
               .ThenInclude(t => t.OrderedPrice)

               );
            AddInclude(o => o.Payments);

        }
    }
}
