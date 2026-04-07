using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification.Admin
{
    internal class AdminGetDestinationByIdSpecification : BaseSpecification<Destination, Guid>
    {
        public AdminGetDestinationByIdSpecification(Guid destinationId, GetDestinationByIdQuery requestQuery)
            : base(
                  d => 
                    d.Id == destinationId
            )
        {
            if (requestQuery.Language is null)
            {
                AddInclude(d => d.Translations.Take(1));

            }
            else
            {
                var language = EnumsMapping.ToLanguageEnum(requestQuery.Language);

                AddInclude(d => d.Translations
                    .Where(x => x.Language == language)
                    .Take(1));
            }
        }
    }
}
