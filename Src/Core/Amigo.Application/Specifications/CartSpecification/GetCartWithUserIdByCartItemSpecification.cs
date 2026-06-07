using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.CartSpecification
{
    public class GetCartWithUserIdByCartItemSpecification : BaseSpecification<Cart, Guid>
    {
        public GetCartWithUserIdByCartItemSpecification(string userId)
            : base(c => c.UserId == userId)
        {
            AddInclude(t => t.Items
                    );
                   
                    
            
            
        }

            
    }
}