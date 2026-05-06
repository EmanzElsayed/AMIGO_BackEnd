using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.OrderSpecification
{
    public class GetNotDeletedOrderByIdForAdminSpecification : BaseSpecification<Order, Guid>
    {
        public GetNotDeletedOrderByIdForAdminSpecification(Guid orderId)
            : base(o => o.Id == orderId && !o.IsDeleted)
        {
            AddInclude(t => t
               .Include(t => t.OrderItems)
               .ThenInclude(t => t.OrderedPrice)

               );
            AddInclude(t => t
              .Include(t => t.OrderItems)
              .ThenInclude(t => t.Booking)

              );
            AddInclude(o => o.Payments);
            AddInclude(o => o.User);

        }
    }
    
}
