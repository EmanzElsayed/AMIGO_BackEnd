using Amigo.Application.Validation.Common.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification
{
    public class GetDestinationByIdSpecification : BaseSpecification<Destination, Guid>
    {
        public GetDestinationByIdSpecification(Guid destinationId, bool isAdmin)
            : base(
                 d => d.Id == destinationId
                 && (isAdmin || d.IsActive)

            )
        {
            AddInclude(d => d.Translations);

        }
    }
}
