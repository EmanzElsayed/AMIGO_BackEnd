using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CartSpecification
{
    public class GetCartPriceWithCartItemIdSpecification : BaseSpecification<CartPrice, Guid>
    {
        public GetCartPriceWithCartItemIdSpecification(Guid cartItemId)
            : base(c => !c.IsDeleted && c.CartItemId == cartItemId)
        {
        }
    }
}
