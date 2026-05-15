using Amigo.Application.Mapping;
using Amigo.Application.Validation.Common.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification.User
{
    public class GetAllDestinationSpecification : BaseSpecification<Destination, Guid>
    {
        
        public GetAllDestinationSpecification(GetAllDestinationQuery requestQuery , bool isAdmin,SupportedLanguage language)
            : base( 
                  DestinationCommonSpecification.BuildCriteria(requestQuery,isAdmin,language)

            )
        {
           
            AddInclude(d => d.Translations
                        .OrderByDescending(t => t.Language == language)
                        .Take(1)
                    );
            AddInclude(d => d.Include(c => c.CountryInfo)
            .ThenInclude(c => c.Translations.OrderByDescending(t => t.Language == language)
                        .Take(1)));
            ApplyPagination(requestQuery.PageSize, requestQuery.PageNumber);
        }
    }
}
