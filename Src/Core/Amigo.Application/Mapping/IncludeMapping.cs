using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public class IncludeMapping : IIncludeMapping
    {
        public List<TourIncluded>? TourIncludesToEntity(List<string>? Includes, Tour tour,string language)
        {
            if (Includes is not null && Includes.Any())
            {
                var mappedLanguage = EnumsMapping.ToLanguageEnum(language);
                List<TourIncluded> tourIncludeds = new List<TourIncluded>();
                foreach (var includeValue in Includes)
                {
                    if (!string.IsNullOrWhiteSpace(includeValue))
                    {
                        tourIncludeds.Add(new TourIncluded
                        {
                            Id = Guid.NewGuid(),
                            Tour = tour,
                            TourId = tour.Id,
                            Language = mappedLanguage,
                            Included = includeValue

                        });
                    }
                }
                return tourIncludeds;
            }
            return null;
        }
    }
}
