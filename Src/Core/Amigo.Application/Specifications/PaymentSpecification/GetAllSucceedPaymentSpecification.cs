using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.PaymentSpecification
{
    public class GetAllSucceedPaymentSpecification : BaseSpecification<Payment, Guid>
    {
        public GetAllSucceedPaymentSpecification() 
            : base(p => !p.IsDeleted && p.Status == PaymentStatus.Succeeded)
        {
            AddInclude(p => p.Include(
                    p => p.Order
                ).ThenInclude(o => o.User)

            );
        }
    }
}
