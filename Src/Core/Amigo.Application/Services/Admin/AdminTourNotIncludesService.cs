using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminTourNotIncludesService :IAdminTourNotIncludesService
    {
        public Task UpdateExcludesAsync(
          Tour tour,
          List<string>? excludes,
          Language? language)
        {
            if (excludes is null || excludes.Count == 0 || language is null)
                return Task.CompletedTask;



            var existingExcludes = tour.NotIncluded
                .Where(e => e.Language == language)
                .ToList();

            foreach (var item in existingExcludes)
                tour.NotIncluded.Remove(item);

            foreach (var exclude in excludes)
            {
                tour.NotIncluded.Add(new TourNotIncluded
                {
                    NotIncluded = exclude,
                    Language = language.Value,
                    TourId = tour.Id
                });
            }

            return Task.CompletedTask;
        }
    }
}

