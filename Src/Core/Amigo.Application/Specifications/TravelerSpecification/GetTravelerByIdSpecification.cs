using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.TravelerSpecification
{
    public class GetTravelerByIdSpecification : BaseSpecification<Traveler, Guid>
    {
        public GetTravelerByIdSpecification(string userId , Guid travelerId)
            : base(t => !t.IsDeleted && t.Booking.UserId == userId && t.Id == travelerId)
        {
        }
    }
}
