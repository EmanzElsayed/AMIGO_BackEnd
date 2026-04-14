using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Mapping
{
    public class InclusionMapping : IInclusionMapping
    {
        public List<TourInclusion>? TourInclusionToEntity(
             List<string>? includedList,
             List<string>? excludedList,
             Tour tour,
             string language
             )
        {
            if (tour == null || string.IsNullOrWhiteSpace(language) )
                return null;


            var mappedLanguage = EnumsMapping.ToLanguageEnum(language);

            var allInputs = new List<(string Text, bool IsIncluded)>();

            if (includedList is not null && includedList.Any())
            {
                allInputs.AddRange(
                    includedList
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => (x.Trim(), true))
                );
            }

            if (excludedList is not null && excludedList.Any())
            {
                allInputs.AddRange(
                    excludedList
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => (x.Trim(), false))
                );
            }

            if (!allInputs.Any())
                return null;


            return allInputs
                .Where(include => !string.IsNullOrWhiteSpace(include.Text))
                .Select(include => new TourInclusion
                {
                    Id = Guid.NewGuid(),
                    Tour = tour,
                    TourId = tour.Id,
                    IsIncluded = include.IsIncluded,
                    Translations = new List<InclusionTranslation>
                    {
                        new InclusionTranslation
                        {
                            Id = Guid.NewGuid(),
                            Text = include.Text,
                            Language = mappedLanguage
                        }
                    }
                })
                .ToList();
        }
    }
}
