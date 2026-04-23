using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification
{
    public class GetNotDeletedDestinationByIdSpecification : UserBaseSpecification<Destination, Guid>
    {
        public GetNotDeletedDestinationByIdSpecification(Guid id)
            : base(d => d.Id == id && d.IsDeleted == false)
        {
        }
    }
}
