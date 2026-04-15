using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminTourInclusionService(IUnitOfWork _unitOfWork) : IAdminTourInclusionService
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

            if (includedList is not null && includedList.Any())
            { 
                var existingInclusions = tour.TourInclusions.Where(i => i.IsIncluded &&  i.Translations.Any(t => t.Language == lang)).ToList();
                if (existingInclusions.Any())
                {
                    _unitOfWork.GetRepository<TourInclusion, Guid>().RemoveRange(existingInclusions);
                }
                
            }

            if (excludedList is not null && excludedList.Any())
            {
                var existingInclusions = tour.TourInclusions.Where(i => !i.IsIncluded && i.Translations.Any(t => t.Language == language)).ToList();
                if (existingInclusions.Any())
                {
                    _unitOfWork.GetRepository<TourInclusion, Guid>().RemoveRange(existingInclusions);
                }

            }
            List<TourInclusion> updateInclustion = new List<TourInclusion>();
            foreach (var input in allInputs)
            {
                updateInclustion.Add(
                new TourInclusion()
                {
                    IsIncluded = input.IsIncluded,
                    Tour = tour,
                    TourId = tour.Id,
                    Translations = new List<InclusionTranslation>
                    { new InclusionTranslation
                        { 
                           Language = lang,
                           Text = input.Text,
                        }
                    
                    }

                }
                 );
                
            }
            tour.TourInclusions = updateInclustion;

            return Task.CompletedTask;
        }
      
    }
}
