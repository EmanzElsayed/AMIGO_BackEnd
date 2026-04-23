using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification.User
{
    public class GetDestinationByIdSpecification : UserBaseSpecification<Destination, Guid>
    {
        public GetDestinationByIdSpecification(Guid destinationId,GetDestinationByIdQuery requestQuery ) 
            : base(
                 DestinationCommonSpecification.BuildGetDestinaionByIdCriteria(requestQuery,destinationId)

            )
        {
            if (requestQuery.Language is null)
            {
                AddInclude(d => d.Translations.Take(1));

            }
            else
            {
                var language = EnumsMapping.ToLanguageEnum(requestQuery.Language);

                AddInclude(d => d.Translations.OrderByDescending(x => x.Language == language).Take(1));
            }
        }
    }
}
