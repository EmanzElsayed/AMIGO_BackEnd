using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.Travelers
{
    public class GetAllDraftTravelersWithItemIds : BaseSpecification<TravelerDraft, Guid>
    {
        public GetAllDraftTravelersWithItemIds(List<Guid> orderitemIds) 
            : base(t => !t.IsDeleted && orderitemIds.Contains(t.OrderItemId))
        {
        }
    }
}
