using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.TravelerSpecification
{
    public class GetTravelerDraftById : BaseSpecification<TravelerDraft, Guid>
    {
        public GetTravelerDraftById(string userId, Guid travelerId)
            : base(t => !t.IsDeleted && t.OrderItem.Order.UserId == userId && t.Id == travelerId)
        {
        }
    }
}
