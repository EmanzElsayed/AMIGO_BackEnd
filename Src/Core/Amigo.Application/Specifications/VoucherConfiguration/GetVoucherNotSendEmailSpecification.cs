using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.VoucherConfiguration
{
    public class GetVoucherNotSendEmailSpecification : BaseSpecification<Voucher, Guid>
    {
        public GetVoucherNotSendEmailSpecification() 
            : base(v => !v.IsDeleted && !v.IsSentByEmail && v.Status == VoucherStatus.Active)
        {
            AddInclude(v => v.Include(v => v.Booking).ThenInclude(b => b.OrderItem));
            AddInclude(v => v.Include(v => v.Booking).ThenInclude(b => b.Travelers));

        }
    }
}
