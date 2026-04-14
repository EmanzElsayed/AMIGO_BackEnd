using Amigo.Domain.DTO.Cancellation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminTourCancellationService :IAdminTourCancellationService
    {
        public Task UpdateCancellationAsync(
              Tour tour,
              UpdateCancellationRequestDTO? dto,
              Language? language)
        {
            if (dto is null)
                return Task.CompletedTask;



            //  ضمان وجود object
            var cancellation = tour.Cancellation ??= new Cancellation
            {
                TourId = tour.Id,
                Translations = new List<CancellationTranslation>()
            };

            //  update basic fields
            if (!string.IsNullOrWhiteSpace(dto.CancelationPolicyType))
                cancellation.CancelationPolicyType =
                    Enum.Parse<CancelationPolicyType>(dto.CancelationPolicyType);

            if (dto.CancellationBefore is not null)
                cancellation.CancellationBefore = dto.CancellationBefore.Value;

            if (dto.RefundPercentage is not null)
                cancellation.RefundPercentage = dto.RefundPercentage.Value;

            //  translation
            if (language is not null &&
                !string.IsNullOrWhiteSpace(dto.Description))
            {


                var translation = cancellation.Translations
                    .FirstOrDefault(t => t.Language == language);

                if (translation is null)
                {
                    cancellation.Translations.Add(new CancellationTranslation
                    {
                        Language = language.Value,
                        Description = dto.Description,
                        Cancellation = cancellation
                    });
                }
                else
                {
                    translation.Description = dto.Description;
                }
            }

            return Task.CompletedTask;
        }
    }
}
