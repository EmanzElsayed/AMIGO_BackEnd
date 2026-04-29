using Amigo.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.TravelerSpecification
{
    public class GetAllDraftTravelersSpecification : BaseSpecification<TravelerDraft, Guid>
    {
        public GetAllDraftTravelersSpecification(GetAllTravelersQuery query,string userId) 
            : base(t => !t.IsDeleted 
            && t.OrderItem.Order.UserId == userId
            && query.Type.Trim().ToLower().Contains(t.Type.Trim().ToLower())
            && (query.CartItemId == null  || t.CartItemId == query.CartItemId)
            && (string.IsNullOrWhiteSpace(query.FullName) || query.FullName.Trim().ToLower().Contains(t.FullName.Trim().ToLower()))     
            )
        {

        }
    }
}
