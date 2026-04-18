using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CartSpecification
{
    public class GetCartItemWithIdSpecification : BaseSpecification<CartItem, Guid>
    {
        public GetCartItemWithIdSpecification(Guid id) 
            : base(c => c.Id == id )
        {
            AddInclude(c => c.Prices);
            AddInclude(c => c.Tour);
        }
    }
}
