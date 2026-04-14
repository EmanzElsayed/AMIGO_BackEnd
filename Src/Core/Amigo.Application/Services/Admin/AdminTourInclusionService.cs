using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminTourInclusionService : IAdminTourInclusionService
    {
      

        public Task UpdateInclusionAsync(
            Tour tour,
            List<string>? includedList,
            List<string>? excludedList,
            Language? language)
        {
            if (tour == null || language is null)
                return Task.CompletedTask;

            var lang = language.Value;

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
                return Task.CompletedTask;

            var existingInclusions = tour.TourInclusions.ToList();

            foreach (var input in allInputs)
            {
                var inclusion = existingInclusions
                    .FirstOrDefault(i => i.IsIncluded == input.IsIncluded);

                if (inclusion is null)
                {
                    inclusion = new TourInclusion
                    {
                        Id = Guid.NewGuid(),
                        TourId = tour.Id,
                        Tour = tour,
                        IsIncluded = input.IsIncluded,
                        Translations = new List<InclusionTranslation>()
                    };

                    tour.TourInclusions.Add(inclusion);
                    existingInclusions.Add(inclusion);
                }

                var translation = inclusion.Translations
                    .FirstOrDefault(t => t.Language == lang && t.Text == input.Text);

                if (translation is null)
                {
                    if (!inclusion.Translations.Any(t => t.Language == lang && t.Text == input.Text))
                    {
                        inclusion.Translations.Add(new InclusionTranslation
                        {
                            Id = Guid.NewGuid(),
                            TourInclusion = inclusion,
                            TourInclusionId = inclusion.Id,
                            Text = input.Text,
                            Language = lang
                        });
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(input.Text))
                        translation.Text = input.Text;
                }
            }

            return Task.CompletedTask;
        }
    }
}
