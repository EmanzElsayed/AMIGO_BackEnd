using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification.Admin
{
    public class GetDestinationWithTranslationsSpecification : BaseSpecification<Destination, Guid>
    {
        public GetDestinationWithTranslationsSpecification(Guid destinationId) 
            : base(d => d.Id == destinationId)
        {
            AddInclude(d => d.Translations);
        }
    }
}
