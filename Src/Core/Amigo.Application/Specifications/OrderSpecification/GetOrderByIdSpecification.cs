using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetOrderByIdSpecification : UserBaseSpecification<Order, Guid>
    {
        public GetOrderByIdSpecification(Guid orderId)
            : base(o => o.Id == orderId && o.IsDeleted == false)
        {
            
            AddInclude(t => t
                  .Include(t => t.OrderItems)
                  .ThenInclude(t => t.OrderedPrice)
                  
                  );
            AddInclude(o => o.User);
        }
    }
}
