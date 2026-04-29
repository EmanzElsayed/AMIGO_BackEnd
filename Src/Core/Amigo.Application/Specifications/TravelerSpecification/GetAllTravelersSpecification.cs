using Amigo.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.TravelerSpecification
{
    public class GetAllTravelersSpecification : BaseSpecification<Traveler, Guid>
    {
        public GetAllTravelersSpecification(GetAllTravelersQuery query , string userId)
             : base(t => !t.IsDeleted
            && t.Booking.UserId == userId
            && query.Type.Trim().ToLower().Contains(t.Type.Trim().ToLower())
            && (string.IsNullOrWhiteSpace(query.FullName) || query.FullName.Trim().ToLower().Contains(t.FullName.Trim().ToLower()))
            )
        {
        }
    }
}
