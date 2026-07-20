using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.TourSpecification.User
{
    public class GetToursByIdsWithNoIncludsSpecification : BaseSpecification<Tour, Guid>
    {
        public GetToursByIdsWithNoIncludsSpecification(List<Guid> tourIds) 
            : base(t => !t.IsDeleted && tourIds.Contains(t.Id))
        {
        }
    }
}
