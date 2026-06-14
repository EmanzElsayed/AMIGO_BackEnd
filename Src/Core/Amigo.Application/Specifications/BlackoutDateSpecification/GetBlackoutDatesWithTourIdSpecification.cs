using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.BlackoutDateSpecification
{
    public class GetBlackoutDatesWithTourIdSpecification : BaseSpecification<BlackoutDate, Guid>
    {
        public GetBlackoutDatesWithTourIdSpecification(Guid tourId) 
            
            : base(d => !d.IsDeleted && d.TourId == tourId)
        {
        }
    }
}
