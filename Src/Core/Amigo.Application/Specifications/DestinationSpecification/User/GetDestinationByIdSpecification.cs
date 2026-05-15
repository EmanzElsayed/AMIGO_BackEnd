using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification.User
{
    public class GetDestinationByIdSpecification : BaseSpecification<Destination, Guid>
    {
        public GetDestinationByIdSpecification(Guid destinationId, SupportedLanguage language ) 
            : base(
                 DestinationCommonSpecification.BuildGetDestinaionByIdCriteria(language, destinationId)

            )
        {
           
               
            AddInclude(d => d.Translations.OrderByDescending(x => x.Language == language).Take(1));
            
            AddInclude(d => d.Include(c => c.CountryInfo).ThenInclude(c => c.Translations.OrderByDescending(x => x.Language == language).Take(1)));

        }
    }
}
