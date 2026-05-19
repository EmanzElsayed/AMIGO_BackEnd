using Amigo.Domain.DTO.Enums;
using Amigo.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.AutoTranslation
{
    public class TourBatchTranslationService(IOpenAiBatchTranslationService _aiService,
                        IUnitOfWork _unitOfWork,
                        ITourTranslationQueryService _queryService)
                : IAutoTranslationService
    {



        public async Task<Result> TranslateAllPendingTours(GetLanguageFromBodyDTO requestDto)
        {

            SupportedLanguage baseLanguage = EnumsMapping.ToLanguageEnum(requestDto.language);
            
            // 1. Get data

            var tours =
                await _queryService
                    .GetPendingTranslationToursAsync(baseLanguage);

            if (!tours.Any())

                return Result.Fail("Not Found Tours");
;

            try {

                // 3. Call OpenAI 
                var result =
                    await _aiService.TranslateToursAsync(tours);

                // 4. Map back to DB
                var tourTranslations = new List<TourTranslation>();
                var destinationTranslation = new List<DestinationTranslation>();
                var cancellationTranslation = new List<CancellationTranslation>();
                var inclustionsTranslation = new List<InclusionTranslation>();
                var pricesTranslation = new List<PriceTranslation>();

                foreach (var lang in result)
                {
                    var language =
                        Enum.Parse<SupportedLanguage>(lang.Language, true);

                    foreach (var tour in lang.Tours)
                    {
                        tourTranslations.Add(new TourTranslation
                        {
                            TourId = tour.TourId,
                            Language = language,
                            Title = tour.Title,
                            Description = tour.Description
                        });


                        // Destination (optional separate table)
                        if (tour.Destination != null)
                        {
                            destinationTranslation.Add(
                                new DestinationTranslation
                                {
                                    DestinationId = tour.Destination.DestinationId,
                                    Language = language,
                                    Name = tour.Destination.Name
                                });
                        }
                        // Cancellation
                        if (tour.Cancellation != null)
                        {
                            cancellationTranslation.Add(
                                new CancellationTranslation
                                {
                                    CancellationId = tour.Cancellation.CancellationId,
                                    Language = language,
                                    Description = tour.Cancellation.Description
                                });
                        }

                        // Inclusions
                        foreach (var inc in tour.Inclusions)
                        {
                            inclustionsTranslation.Add(
                                new InclusionTranslation
                                {
                                    TourInclusionId = inc.InclusionId,
                                    Language = language,
                                    Text = inc.Text
                                });
                        }

                        // Prices
                        foreach (var price in tour.Prices)
                        {
                            pricesTranslation.Add(
                                new PriceTranslation
                                {
                                    PriceId = price.PriceId,
                                    Language = language,
                                    Type = price.Type
                                });
                        }
                    }
                }
                // 5. Bulk save
                await _unitOfWork.GetRepository<TourTranslation, Guid>().AddRangeAsync(tourTranslations);
                await _unitOfWork.GetRepository<CancellationTranslation, Guid>().AddRangeAsync(cancellationTranslation);
                await _unitOfWork.GetRepository<PriceTranslation, Guid>().AddRangeAsync(pricesTranslation);
                await _unitOfWork.GetRepository<DestinationTranslation, Guid>().AddRangeAsync(destinationTranslation);
                await _unitOfWork.GetRepository<InclusionTranslation, Guid>().AddRangeAsync(inclustionsTranslation);

                await _unitOfWork.SaveChangesAsync();

                return Result.Ok().WithSuccess(new Success("Tours Translated Successfully"));

            }
            catch (Exception ex) {
                return FluentValidationExtension.FromException(details: ex.Message);

            }


          
        }

       
    
    }
}