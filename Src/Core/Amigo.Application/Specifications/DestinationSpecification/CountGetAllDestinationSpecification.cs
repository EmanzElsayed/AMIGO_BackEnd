using Amigo.Application.Mapping;
using Amigo.Application.Validation.Common.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification
{
    public class CountGetAllDestinationSpecification : BaseSpecification<Destination, Guid>
    {
        public CountGetAllDestinationSpecification(GetAllDestinationQuery requestQuery ,bool isAdmin)
            : base(
                  DestinationCommonSpecification.BuildCriteria(requestQuery, isAdmin)
            )
        { 
        }
    }
}
