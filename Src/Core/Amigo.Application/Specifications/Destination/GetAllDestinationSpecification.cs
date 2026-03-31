using Amigo.Application.Mapping;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.Destination
{
    public class GetAllDestinationSpecification : BaseSpecification<DestinationTranslation, Guid>
    {
        public GetAllDestinationSpecification(GetAllDestinationQuery requestQuery)
            : base( d =>  
                  (string.IsNullOrWhiteSpace(requestQuery.Name) || d.Name.ToLower().Contains(requestQuery.Name.Trim().ToLower()))
                    && (string.IsNullOrWhiteSpace(requestQuery.Language) || d.Language == EnumsMapping.ToLanguageEnum(requestQuery.Language))
                     && (string.IsNullOrWhiteSpace(requestQuery.CountryCode) || d.Destination.CountryCode == EnumsMapping.ToCountryCodeEnum(requestQuery.CountryCode))

            )
        {
            ApplyPagination(requestQuery.PageSize, requestQuery.PageNumber);
            AddInclude(d => d.Destination);
        }
    }
}
