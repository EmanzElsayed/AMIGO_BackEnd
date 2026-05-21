using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.RefundSpecification
{
    public class GetRefundWithProviderRefundIdSpecification : BaseSpecification<Refund, Guid>
    {
        public GetRefundWithProviderRefundIdSpecification(string providerRefundId) 
            : base(r => !r.IsDeleted && r.ProviderRefundId == providerRefundId)
        {
            AddInclude(r => r.Booking);
        }
    }
}
