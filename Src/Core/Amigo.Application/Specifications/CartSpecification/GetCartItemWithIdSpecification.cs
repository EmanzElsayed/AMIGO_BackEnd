using Microsoft.EntityFrameworkCore;
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
            AddInclude(q => q
                .Include(c => c.Prices)
                .Include(c => c.Travelers)
                .Include(c => c.Tour)
                .ThenInclude(t => t.Prices)
                .ThenInclude(p => p.Translations)
            );
        }
    }
}
