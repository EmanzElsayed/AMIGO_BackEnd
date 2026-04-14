using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminTourIncludesService : IAdminTourIncludesService
    {
        public Task UpdateIncludesAsync(
            Tour tour,
            List<string>? includes,
            Language? language)
        {
            if (includes is null || includes.Count == 0 || language is null)
                return Task.CompletedTask;



            //  كل includes بنفس اللغة
            var existingIncludes = tour.Included
                .Where(i => i.Language == language)
                .ToList();

            //  strategy: replace language بالكامل
            //  remove القديم
            foreach (var item in existingIncludes)
                tour.Included.Remove(item);

            //  add الجديد
            foreach (var include in includes)
            {
                tour.Included.Add(new TourIncluded
                {
                    Included = include,
                    Language = language.Value,
                    TourId = tour.Id
                });
            }

            return Task.CompletedTask;
        }
    }
}
