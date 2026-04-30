using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.CartSpecification
{
    public class GetCartWithUserIdSpecification : BaseSpecification<Cart, Guid>
    {
        public GetCartWithUserIdSpecification(string userId) 
            : base( c => c.UserId == userId )
        {

            AddInclude(t => t
                    .Include(t => t.Items)
                    .ThenInclude(t => t.Prices)
                    .Include(t => t.Items)
                    .ThenInclude(t => t.Travelers)
                    .Include(t => t.Items)
                    .ThenInclude(t => t.Tour)
                    .ThenInclude(t => t.Prices)
                    .ThenInclude(t => t.Translations)
                    );


        }
    }
}
