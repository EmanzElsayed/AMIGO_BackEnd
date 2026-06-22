using Amigo.Application.Specifications.CountriesInfo;
using Amigo.Domain.DTO.Enums;
using Amigo.Domain.DTO.Translation;
using Amigo.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using PayPalCheckoutSdk.Orders;
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
        public async Task<Result> TranslateDestination(SupportedLanguage sourceLanguage, DestinationTranslationItem destinationTranslationItem,Guid countryId)
        {
            if (destinationTranslationItem is null)
            {
                return Result.Fail("Not Found Tour");
            }
            try
            {
                // 3. Call OpenAI 
                var result =
                    await _aiService.TranslateDestinationAsync(destinationTranslationItem, sourceLanguage);
                // 4. Map back to DB

                var destinationTranslation = new List<DestinationTranslation>();


                var existingDestination = await _unitOfWork
                   .GetRepository<DestinationTranslation, Guid>()
                   .GetAllWithTrackingAsync();
                var countryInfoTr = await _unitOfWork.GetRepository<CountryInfoTranslation, Guid>().GetAllAsync(new GetAllTranslationWithCountryIdSpecifciation(countryId));
                var destinationMap = existingDestination
                 .ToDictionary(x => (x.DestinationId, x.Language));

                foreach (var lang in result)
                {
                    var language =
                        Enum.Parse<SupportedLanguage>(lang.Language, true);




                    if (lang.Destination != null)
                    {
                        if (destinationMap.TryGetValue((lang.Destination.DestinationId, language), out var existingDest))
                        {
                            existingDest.Name = !string.IsNullOrWhiteSpace(lang.Destination.Name) ? lang.Destination.Name : existingDest.Name;
                        }
                        else
                        {
                            destinationTranslation.Add(
                           new DestinationTranslation
                           {
                               DestinationId = lang.Destination.DestinationId,
                               Language = language,
                               Name = lang.Destination.Name
                           });
                        }
                        if (countryInfoTr is not null && countryInfoTr.Any() && !string.IsNullOrWhiteSpace(lang.Destination.CountryDescription))
                        { 
                            var countryWithLang = countryInfoTr.Where(tr => tr.Language == language).FirstOrDefault();
                            if (countryWithLang != null) countryWithLang.Description = lang.Destination.CountryDescription;

                        }
                    }

                }
                if (destinationTranslation.Any())

                    await _unitOfWork.GetRepository<DestinationTranslation, Guid>().AddRangeAsync(destinationTranslation);
                await _unitOfWork.SaveChangesAsync();

                return Result.Ok().WithSuccess(new Success("Destination Translated Successfully"));

            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);

            }
        }

        public async Task<Result> TranslateTour(SupportedLanguage sourceLanguage , TourTranslationItem tourTranslationItem)
        {
            if (tourTranslationItem is null)
            {
                return Result.Fail("Not Found Tour");
            }

            try
            {
                // 3. Call OpenAI 
                var result =
                    await _aiService.TranslateTourAsync(tourTranslationItem,sourceLanguage);
                // 4. Map back to DB
                var tourTranslations = new List<TourTranslation>();
               
                //var cancellationTranslation = new List<CancellationTranslation>();
                var inclustionsTranslation = new List<InclusionTranslation>();
                var pricesTranslation = new List<PriceTranslation>();

                var existingTours = await _unitOfWork
                    .GetRepository<TourTranslation, Guid>()
                    .GetAllWithTrackingAsync();

                var tourMap = existingTours
                .ToDictionary(x => (x.TourId, x.Language));



                //var existingCancellation = await _unitOfWork
                // .GetRepository<CancellationTranslation, Guid>()
                // .GetAllWithTrackingAsync();

                //var cancellationMap = existingCancellation
                // .ToDictionary(x => (x.CancellationId, x.Language));

                var existingPrice = await _unitOfWork
               .GetRepository<PriceTranslation, Guid>()
               .GetAllWithTrackingAsync();

                var priceMap = existingPrice
                 .ToDictionary(x => (x.PriceId, x.Language));

                var existingInclustion = await _unitOfWork
              .GetRepository<InclusionTranslation, Guid>()
              .GetAllWithTrackingAsync();

                var inclustionMap = existingInclustion
                 .ToDictionary(x => (x.TourInclusionId, x.Language));


                foreach (var lang in result)
                {
                    var language =
                        Enum.Parse<SupportedLanguage>(lang.Language, true);


                        if (tourMap.TryGetValue((lang.Tour.TourId, language), out var existing))
                        {
                            // UPDATE
                            existing.Title = !string.IsNullOrWhiteSpace(lang.Tour.Title) ? lang.Tour.Title : existing.Title;
                            existing.Description = !string.IsNullOrWhiteSpace(lang.Tour.Description) ? lang.Tour.Description : existing.Description;
                        }
                        else
                        {
                            tourTranslations.Add(new TourTranslation
                            {
                                TourId = lang.Tour.TourId,
                                Language = language,
                                Title = lang.Tour.Title,
                                Description = lang.Tour.Description
                            });
                        }




                       


                        //// Cancellation
                        //if (lang.Tour.Cancellation != null)
                        //{
                        //    if (cancellationMap.TryGetValue((lang.Tour.Cancellation.CancellationId, language), out var exisingCancellation))
                        //    {
                        //        exisingCancellation.Description = !string.IsNullOrWhiteSpace(lang.Tour.Cancellation.Description) ? lang.Tour.Cancellation.Description : exisingCancellation.Description;
                        //    }
                        //    else
                        //    {
                        //        cancellationTranslation.Add(
                        //              new CancellationTranslation
                        //              {
                        //                  CancellationId = lang.Tour.Cancellation.CancellationId,
                        //                  Language = language,
                        //                  Description = lang.Tour.Cancellation.Description
                        //              });
                        //    }


                        //}

                    // Inclusions
                    if (lang.Tour.Inclusions is not null)
                    {
                        foreach (var inc in lang.Tour.Inclusions)
                        {
                            if (inclustionMap.TryGetValue((inc.InclusionId, language), out var inclustion))
                            {
                                inclustion.Text = !string.IsNullOrWhiteSpace(inc.Text) ? inc.Text : inclustion.Text;
                            }
                            else
                            {
                                inclustionsTranslation.Add(
                                        new InclusionTranslation
                                        {
                                            TourInclusionId = inc.InclusionId,
                                            Language = language,
                                            Text = inc.Text
                                        });
                            }

                        }
                    }


                    // Prices
                    if (lang.Tour.Prices is not null)
                    {
                        foreach (var price in lang.Tour.Prices)
                        {
                            if (priceMap.TryGetValue((price.PriceId, language), out var exPrice))
                            {
                                exPrice.ActivityType = !string.IsNullOrWhiteSpace(price.ActivityType) ? price.ActivityType : exPrice.ActivityType;
                                exPrice.Type = !string.IsNullOrWhiteSpace(price.Type) ? price.Type : exPrice.Type;
                            }
                            else
                            {
                                pricesTranslation.Add(
                                    new PriceTranslation
                                    {
                                        PriceId = price.PriceId,
                                        Language = language,
                                        Type = price.Type,
                                        ActivityType = price.ActivityType

                                    });
                            }

                        }
                    }
                        
                    
                }
                // 5. Bulk save
                if (tourTranslations.Any())
                    await _unitOfWork.GetRepository<TourTranslation, Guid>().AddRangeAsync(tourTranslations);
                //if (cancellationTranslation.Any())

                //    await _unitOfWork.GetRepository<CancellationTranslation, Guid>().AddRangeAsync(cancellationTranslation);
                if (pricesTranslation.Any())

                    await _unitOfWork.GetRepository<PriceTranslation, Guid>().AddRangeAsync(pricesTranslation);
                  
                if (inclustionsTranslation.Any())

                    await _unitOfWork.GetRepository<InclusionTranslation, Guid>().AddRangeAsync(inclustionsTranslation);

                await _unitOfWork.SaveChangesAsync();

                return Result.Ok().WithSuccess(new Success("Tours Translated Successfully"));

            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);

            }
        }

        public async Task<Result> TranslateAllPendingTours(GetLanguageFromBodyDTO requestDto)
        {

            SupportedLanguage baseLanguage = EnumsMapping.ToLanguageEnum(requestDto.Language);
            
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

                var existingTours = await _unitOfWork
                    .GetRepository<TourTranslation, Guid>()
                    .GetAllWithTrackingAsync();

                var tourMap = existingTours
                .ToDictionary(x => (x.TourId, x.Language));


                var existingDestination = await _unitOfWork
                   .GetRepository<DestinationTranslation, Guid>()
                   .GetAllWithTrackingAsync();

                var destinationMap = existingDestination
                 .ToDictionary(x => ( x.DestinationId, x.Language ));

                var existingCancellation = await _unitOfWork
                 .GetRepository<CancellationTranslation, Guid>()
                 .GetAllWithTrackingAsync();

                var cancellationMap = existingCancellation
                 .ToDictionary(x => (x.CancellationId, x.Language ));

                var existingPrice = await _unitOfWork
               .GetRepository<PriceTranslation, Guid>()
               .GetAllWithTrackingAsync();

                var priceMap = existingPrice
                 .ToDictionary(x => ( x.PriceId, x.Language ));

                var existingInclustion = await _unitOfWork
              .GetRepository<InclusionTranslation, Guid>()
              .GetAllWithTrackingAsync();

                var inclustionMap = existingInclustion
                 .ToDictionary(x => ( x.TourInclusionId, x.Language ));


                foreach (var lang in result)
                {
                    var language =
                        Enum.Parse<SupportedLanguage>(lang.Language, true);

                    foreach (var tour in lang.Tours)
                    {

                        if ( tourMap.TryGetValue((tour.TourId, language), out var existing))
                        {
                            // UPDATE
                            existing.Title = tour.Title;
                            existing.Description = tour.Description;
                        }
                        else
                        {
                            tourTranslations.Add(new TourTranslation
                            {
                                TourId = tour.TourId,
                                Language = language,
                                Title = tour.Title,
                                Description = tour.Description
                            });
                        }





                        
                       
                        //// Cancellation
                        //if (tour.Cancellation != null)
                        //{
                        //    if (cancellationMap.TryGetValue((tour.Cancellation.CancellationId, language), out var exisingCancellation))
                        //    {
                        //        exisingCancellation.Description = tour.Cancellation.Description;
                        //    }
                        //    else
                        //    {
                        //        cancellationTranslation.Add(
                        //              new CancellationTranslation
                        //              {
                        //                  CancellationId = tour.Cancellation.CancellationId,
                        //                  Language = language,
                        //                  Description = tour.Cancellation.Description
                        //              });
                        //    }

                              
                        //}

                        // Inclusions
                        foreach (var inc in tour.Inclusions)
                        {
                            if (inclustionMap.TryGetValue((inc.InclusionId, language), out var inclustion))
                            {
                                inclustion.Text = inc.Text;
                            }
                            else
                            {
                                inclustionsTranslation.Add(
                                        new InclusionTranslation
                                        {
                                            TourInclusionId = inc.InclusionId,
                                            Language = language,
                                            Text = inc.Text
                                        });
                            }
                                
                        }

                        // Prices
                        foreach (var price in tour.Prices)
                        {
                            if (priceMap.TryGetValue((price.PriceId, language), out var exPrice))
                            {
                                exPrice.ActivityType = price.ActivityType;
                                exPrice.Type = price.Type;
                            }
                            else
                            {
                                pricesTranslation.Add(
                                    new PriceTranslation
                                    {
                                        PriceId = price.PriceId,
                                        Language = language,
                                        Type = price.Type,
                                        ActivityType = price.ActivityType

                                    });
                            }
                                
                        }
                    }
                }
                // 5. Bulk save
                if(tourTranslations.Any())
                     await _unitOfWork.GetRepository<TourTranslation, Guid>().AddRangeAsync(tourTranslations);
                //if(cancellationTranslation.Any())

                //    await _unitOfWork.GetRepository<CancellationTranslation, Guid>().AddRangeAsync(cancellationTranslation);
                if (pricesTranslation.Any())

                    await _unitOfWork.GetRepository<PriceTranslation, Guid>().AddRangeAsync(pricesTranslation);
                if (destinationTranslation.Any())

                    await _unitOfWork.GetRepository<DestinationTranslation, Guid>().AddRangeAsync(destinationTranslation);
                if (inclustionsTranslation.Any())

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