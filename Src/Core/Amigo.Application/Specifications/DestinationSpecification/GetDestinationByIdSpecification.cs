using Amigo.Application.Mapping;
using Amigo.Application.Validation.Common.Specifications;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification
{
    public class GetDestinationByIdSpecification : BaseSpecification<Destination, Guid>
    {
        public GetDestinationByIdSpecification(Guid destinationId, bool isAdmin , GetDestinationByIdQuery requestQuery)
            : base( d =>
                   d.Id == destinationId


                && (isAdmin || d.IsActive)

            )
        {
            if (requestQuery.Language is not null)
            {
                var language = EnumsMapping.ToLanguageEnum(requestQuery.Language);


                if (isAdmin)
                {
                    // Admin: يرجع فقط اللغة المطلوبة أو null لو مش موجودة
                    AddInclude(d => d.Translations
                        .Where(t => t.Language == language)
                    );
                }
                else
                {
                    // User: يرجع اللغة المطلوبة لو موجودة، أو أي ترجمة واحدة متاحة
                    AddInclude(d => d.Translations
                        .OrderByDescending(t => t.Language == language)
                        .Take(1)
                    );
                }
            }
            else
            {
                AddInclude(d => d.Translations);
            }

        }
    }
}
