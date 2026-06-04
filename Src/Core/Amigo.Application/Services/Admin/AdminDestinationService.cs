

using Amigo.Application.BackgroundTasks;
using Amigo.Application.Specifications.CountriesInfo;
using Amigo.Domain.DTO.Translation;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Amigo.Application.Services.Admin
{
    public class AdminDestinationService(IValidationService _validationService,
                                    IUnitOfWork _unitOfWork,
                                    ICurrentUserService _currentUserService,
                                    ImageCloudService _imageCloud,
                                    ILogger<AdminDestinationService> _logger,
                                    IBackgroundTaskQueue _backgroundTaskQueue) : IAdminDestinationService
    {
        public async Task<Result> CreateDestinationAsync(CreateDestinationRequestDTO requestDTO)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);

            if (!validationResult.IsSuccess)
                return validationResult;

            CountryCode countryCode =
                EnumsMapping.ToCountryCodeEnum(requestDTO.CountryCode);

            try
            {
                SupportedLanguage requestLanguage = EnumsMapping.ToLanguageEnum(requestDTO.Language);
                var countryInfoRepo = _unitOfWork.GetRepository<CountryInfo, Guid>();
                var countryInfo =
                    await countryInfoRepo
                        .GetByIdAsync(
                            new GetCountryByCountryCodeSpecification(
                                countryCode,
                                requestLanguage));

                if (countryInfo is null)
                    return Result.Fail(new NotFoundError("This Country Not Found"));

                var destination = new Destination
                {
                    IsActive = requestDTO.IsActive ?? true,
                    ImageUrl = requestDTO.ImageUrl,
                    ImagePublicId = requestDTO.PublicId,
                    CountryInfoId = countryInfo.Id,
                    CountryInfo = countryInfo
                };

                await _unitOfWork
                    .GetRepository<Destination, Guid>()
                    .AddAsync(destination);

                await _unitOfWork.SaveChangesAsync();

                var destinationTranslation = new DestinationTranslation
                {
                    Name = requestDTO.Name,
                    Language = requestLanguage,
                    DestinationId = destination.Id
                };

                await _unitOfWork
                    .GetRepository<DestinationTranslation, Guid>()
                    .AddAsync(destinationTranslation);

                await _unitOfWork.SaveChangesAsync();

               
                var sourceLanguage = EnumsMapping.ToLanguageEnum(requestDTO.Language);
                var inputTranslate = new DestinationTranslationItem()
                {
                    SourceLanguage = sourceLanguage,
                    DestinationId = destination.Id,
                    Name = requestDTO.Name
                };
                await _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
                {
                    var autoTranslationService =
                           serviceProvider.GetRequiredService<IAutoTranslationService>();

                    await autoTranslationService.TranslateDestination(
                        sourceLanguage,
                        inputTranslate);
                    //await TranslateDestinationInBackgroundAsync(destinationId, originalName, requestLanguage, serviceProvider);
                });

                return Result.Ok()
                    .WithSuccess(new Success("Destination Created Successfully")
                    .WithMetadata("StatusCode", (int)HttpStatusCode.Created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating destination");
                return FluentValidationExtension.FromException(details: ex.Message);
            }
        }

        private async Task TranslateDestinationInBackgroundAsync(
    Guid destinationId,
    string originalName,
    SupportedLanguage requestLanguage,
    IServiceProvider serviceProvider)
        {
            try
            {
                using var scope =
                    serviceProvider.CreateScope();

                var unitOfWork =
                    scope.ServiceProvider
                         .GetRequiredService<IUnitOfWork>();

                var translationService =
                    scope.ServiceProvider
                         .GetRequiredService<ITranslationService>();

                var logger =
                    scope.ServiceProvider
                         .GetRequiredService<
                             ILogger<AdminDestinationService>>();

                var languageMap =
                    new Dictionary<SupportedLanguage, string>
                    {
                { SupportedLanguage.en, "English" },
                { SupportedLanguage.es, "Spanish" },
                { SupportedLanguage.fr, "French" },
                { SupportedLanguage.it, "Italian" },
                { SupportedLanguage.pt, "Portuguese" },
                { SupportedLanguage.br, "Brazilian Portuguese" }
                    };

                var targetLanguages =
                    languageMap
                        .Where(x => x.Key != requestLanguage)
                        .ToList();

                var translations =
                    await translationService.TranslateAsync(
                        originalName,
                        targetLanguages
                            .Select(x => x.Value)
                            .ToList());

                foreach (var language in targetLanguages)
                {
                    if (!translations.TryGetValue(
                            language.Value,
                            out var translatedName))
                    {
                        logger.LogWarning(
                            "Missing translation for {Language}",
                            language.Value);

                        continue;
                    }

                    var destinationTranslation =
                        new DestinationTranslation
                        {
                            Name = translatedName,
                            Language = language.Key,
                            DestinationId = destinationId
                        };

                    await unitOfWork
                        .GetRepository<
                            DestinationTranslation,
                            Guid>()
                        .AddAsync(destinationTranslation);
                }

                await unitOfWork.SaveChangesAsync();

                logger.LogInformation(
                    "Destination {DestinationId} translated successfully",
                    destinationId);
            }
            catch (Exception ex)
            {
                var logger =
                    serviceProvider
                        .GetRequiredService<
                            ILogger<AdminDestinationService>>();

                logger.LogError(
                    ex,
                    "Error translating destination {DestinationId}",
                    destinationId);
            }
        }
        public async Task<Result> UpdateDestination(UpdateDestinationRequestDTO requestDTO, string Id)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid destinationId = guid;

            var _destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();
            var _bookingRepo = _unitOfWork.GetRepository<Booking, Guid>();


            var destination = await _destinationRepo.GetByIdAsync(new GetDestinationWithTranslationsSpecification(destinationId));

            if (destination is null)
            {
                return Result.Fail(new NotFoundError("This Destination Not Found"));
            }
            DestinationTranslation? translation = null;
            SupportedLanguage? languageEnum = null;

            if (requestDTO.Language is not null)
            {
                languageEnum = EnumsMapping.ToLanguageEnum(requestDTO.Language);
                translation = destination.Translations
                             .FirstOrDefault(t => t.Language == languageEnum);
            }


            


            if (!string.IsNullOrWhiteSpace(requestDTO.CountryCode) && languageEnum is not null)
            { 
                CountryCode countryCode = EnumsMapping.ToCountryCodeEnum(requestDTO.CountryCode);
                var countryInfo = await _unitOfWork.GetRepository<CountryInfo, Guid>().GetByIdAsync(new GetCountryByCountryCodeSpecification(countryCode, languageEnum.Value));

                if (countryInfo is null)
                {
                    return Result.Fail(new NotFoundError("This County Not Found"));
                }
                destination.CountryInfoId = countryInfo.Id;
                destination.CountryInfo = countryInfo;

            }


            // image logic
            if (!string.IsNullOrWhiteSpace(requestDTO.ImageUrl) )
            {
                destination.ImageUrl = requestDTO.ImageUrl;

                if (destination.ImagePublicId is not null)
                    _imageCloud.DeleteImage(destination.ImagePublicId);

                if (requestDTO.PublicId is not null)
                    destination.ImagePublicId = requestDTO.PublicId;
            }

            DestinationMapping.UpdateDestination(requestDTO, destination, translation, languageEnum);

            try
            {
                await _unitOfWork.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(requestDTO.Language))
                {
                    var sourceLanguage = EnumsMapping.ToLanguageEnum(requestDTO.Language);
                    var inputTranslate = new DestinationTranslationItem()
                    {
                        SourceLanguage = sourceLanguage,
                        DestinationId = destination.Id,
                        Name = requestDTO.Name ?? ""
                    };
                    _ = _backgroundTaskQueue.EnqueueAsync(async (serviceProvider, cancellationToken) =>
                    {

                        var autoTranslationService =
                           serviceProvider.GetRequiredService<IAutoTranslationService>();

                        await autoTranslationService.TranslateDestination(
                            sourceLanguage,
                            inputTranslate);


                    });
                }
                return Result.Ok()
                               .WithSuccess(new Success("Destination Updated Successfully"));
            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);

            }

        }
        public async Task<Result> UpdateActivationDestinaion(UpdateActivationDestinationRequestDTO requestDTO, string Id)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid destinationId = guid;

            var _destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();

            var destination = await _destinationRepo.GetByIdAsync(destinationId);

            if (destination is null)
            {
                return Result.Fail(new NotFoundError("This Destination Not Found"));
            }
            if (destination.IsActive == requestDTO.IsActive)
            {
                return Result.Ok()
                              .WithSuccess(new Success("Activation Status Already Updated Successfully"));

            }
            destination.IsActive = requestDTO.IsActive;

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return Result.Ok()
                               .WithSuccess(new Success("Activation Status Updated Successfully"));
            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);

            }
        }

        public async Task<Result> DeleteDestination(string Id)
        {
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid destinationId = guid;

            var _destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();

            var destination = await _destinationRepo.GetByIdAsync(destinationId);


            if (destination is null)
            {
                return Result.Fail(new NotFoundError("This Destination Not Found"));
            }

            destination.SetIsDeleted(true);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                
                return Result.Ok()
                               .WithSuccess(new Success($"Destination Deleted Successfully"));
            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);

            }
        }


        public async Task<Result<PaginatedResponse<GetDestinationResponseDTO>>> GetAllDestinationAsync(GetAllDestinationQuery requestQuery)
        {
            var validationResult = await _validationService.ValidateAsync(requestQuery);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            var destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();

            SupportedLanguage language = _currentUserService.Language;
            var destinationSpecification = new GetAllDestinationSpecification(requestQuery, true, language);
            var destinationData = await destinationRepo.GetAllAsync(destinationSpecification);

            var countDestinationSpecification = new CountGetAllDestinationSpecification(requestQuery, true, language);
            var countDestinationData = await destinationRepo.GetCountSpecificationAsync(countDestinationSpecification);

            
            var mappedDestinationData = DestinationMapping.EntitiesToDestinations(destinationData,language);
            var paginatedResult = new PaginatedResponse<GetDestinationResponseDTO>
            {
                Data = mappedDestinationData,
                PageNumber = requestQuery.PageNumber,
                PageSize = requestQuery.PageSize,
                TotalItems = countDestinationData
            };
            return Result.Ok(paginatedResult);
        }

        public async Task<Result<GetDestinationResponseDTO>> GetDestinationByIdAsync(string Id, GetLanuageQuery requestQuery)
        {
            var validationResult = await _validationService.ValidateAsync(requestQuery);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid destinationId = guid;

            var destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();
            var destinationSpecification = new AdminGetDestinationByIdSpecification(destinationId, requestQuery);

            var destinationData = await destinationRepo.GetByIdAsync(destinationSpecification);
            if (destinationData is null)
            {
                return Result.Fail(new NotFoundError($"This Destination Not Found"));
            }
            SupportedLanguage? language = string.IsNullOrWhiteSpace(requestQuery.Language) ? null : EnumsMapping.ToLanguageEnum(requestQuery.Language);

            var mappedDestinationData = DestinationMapping.EntityToAdminDestination(destinationData, language);
            return Result.Ok(mappedDestinationData);
        }

       
    }
}
