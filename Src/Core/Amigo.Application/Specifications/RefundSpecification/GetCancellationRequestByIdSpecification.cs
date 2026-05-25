using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.RefundSpecification
{
    public class GetCancellationRequestByIdSpecification : BaseSpecification<CancellationRequest, Guid>
    {
        public GetCancellationRequestByIdSpecification(Guid requestId) 
            : base(r => !r.IsDeleted && r.Id == requestId)
        {
            AddInclude(c => c.Include(b => b.Booking).ThenInclude(i => i.OrderItem));
        }
    }
}
