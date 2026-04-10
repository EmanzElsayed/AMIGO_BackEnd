using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public class NotIncludedMapping : INotIncludedMapping
    {
        public List<TourNotIncluded>? TourNotIncludesToEntity(List<string> NotIncludes, Tour tour, string language)
        {
            if (NotIncludes is not null && NotIncludes.Any())
            {
                var mappedLanguage = EnumsMapping.ToLanguageEnum(language);
                List<TourNotIncluded> tourNotIncludeds = new List<TourNotIncluded>();
                foreach (var includeValue in NotIncludes)
                {
                    if (!string.IsNullOrWhiteSpace(includeValue))
                    {
                        tourNotIncludeds.Add(new TourNotIncluded
                        {
                            Id = Guid.NewGuid(),
                            Tour = tour,
                            TourId = tour.Id,
                            Language = mappedLanguage,
                            NotIncluded = includeValue

                        });
                    }
                }
                return tourNotIncludeds;
            }
            return null;
        }
    }
}
