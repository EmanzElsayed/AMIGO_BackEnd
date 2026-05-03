using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.VoucherConfiguration
{
    public class GetVoucherWithTokenSpecification : BaseSpecification<Voucher, Guid>
    {
        public GetVoucherWithTokenSpecification(string token)
            : base(v => !v.IsDeleted && v.Token.Contains(token))
        {
        }
    }
}
