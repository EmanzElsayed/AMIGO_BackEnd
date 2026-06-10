using Amigo.Application.Validation.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Specifications.DestinationSpecification.Admin
{
    internal class AdminGetDestinationByIdSpecification : BaseSpecification<Destination, Guid>
    {
        public AdminGetDestinationByIdSpecification(Guid destinationId, GetLanuageQuery requestQuery, SupportedLanguage? language)
            : base(
                  d => 
                    d.Id == destinationId && !d.IsDeleted
            )
        {
            AddInclude(d => d.Include(c => c.CountryInfo).ThenInclude(c => c.Translations));

            if (language == null)
            {
                AddInclude(d => d.Translations.Take(1));

            }
            else
            {

                AddInclude(d => d.Translations
                    .Where(x => x.Language == language)
                    .Take(1));
            }
        }
    }
}
