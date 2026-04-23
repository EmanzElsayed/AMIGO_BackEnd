using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.AvailableSlotsSpecification
{
    public class GetAvaialableSlotsByIdSpecification : UserBaseSpecification<AvailableSlots, Guid>
    {
        public GetAvaialableSlotsByIdSpecification(Guid id) 
            : base(a => a.Id == id && a.IsDeleted == false)
        {
        }
    }
}
