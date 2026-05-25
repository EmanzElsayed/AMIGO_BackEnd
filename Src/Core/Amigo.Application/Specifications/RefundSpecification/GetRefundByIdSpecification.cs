using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.RefundSpecification
{
    public class GetRefundByIdSpecification : BaseSpecification<Refund, Guid>
    {
        public GetRefundByIdSpecification(Guid refundId) 
            : base(r => !r.IsDeleted && r.Id == refundId)
        {
            AddInclude(r => r.Include(b => b.Booking).ThenInclude(i => i.OrderItem));
            AddInclude(r => r.Payment);
        }
    }
}
