using Amigo.Application.Mapping;
using Amigo.Application.Validation.Common.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification
{
    public class GetAllDestinationSpecification : BaseSpecification<Destination, Guid>
    {
        
        public GetAllDestinationSpecification(GetAllDestinationQuery requestQuery , bool isAdmin)
            : base( 
                  DestinationCommonSpecification.BuildCriteria(requestQuery,isAdmin)

            )
        {
            if (requestQuery.Language is not null)
            {
                var language = EnumsMapping.ToLanguageEnum(requestQuery.Language);

                AddInclude(d => d.Translations
                    .OrderByDescending(t => t.Language == language) // اللغة المطلوبة أول
                    .Take(1) // لو مش موجودة، هايجيب أول ترجمة متاحة
                );
            }
            else
            {
                AddInclude(d => d.Translations);
            }
            ApplyPagination(requestQuery.PageSize, requestQuery.PageNumber);
        }
    }
}
