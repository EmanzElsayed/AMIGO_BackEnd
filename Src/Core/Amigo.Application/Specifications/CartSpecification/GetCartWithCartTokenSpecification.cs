using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.CartSpecification
{
    public class GetCartWithCartTokenSpecification : UserBaseSpecification<Cart, Guid>
    {
        public GetCartWithCartTokenSpecification(string cartToken)
            : base(c => c.CartToken == cartToken)
        {

            AddInclude(t => t
                   .Include(t => t.Items)
                   .ThenInclude(t => t.Prices)
                   );


        }
    
    }
}
