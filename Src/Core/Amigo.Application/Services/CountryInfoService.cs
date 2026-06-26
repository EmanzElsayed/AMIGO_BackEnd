using Amigo.Application.Abstraction.Services;
using Amigo.Application.Specifications.CountriesInfo;
using Amigo.Domain.DTO.CountryInfo;
using FluentValidation.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class CountryInfoService(IValidationService _validationService,IUnitOfWork _unitOfWork) 
        : ICountryInfoService
    {
        public async Task<Result<List<GetCountryInfoResponseDTO>>> GetAllCountryInfoAsync(GetAllCountryInfoQuery requestQuery)
        {
            var validationResult = await _validationService.ValidateAsync(requestQuery);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            SupportedLanguage language = !string.IsNullOrWhiteSpace( requestQuery.Language)  ? EnumsMapping.ToLanguageEnum(requestQuery.Language) : SupportedLanguage.en;

            var countrySpec = new GetAllCountryInfoSpecification(requestQuery, language);
            var CountriesInfo = await _unitOfWork.GetRepository<CountryInfo, Guid>().GetAllAsync(countrySpec);

            var mappedCountriesInfo = CountriesInfo.FromEntitiesToDTOs();

            return Result.Ok(mappedCountriesInfo);

        }

        public async Task<Result<CountryDescriptionResponseDTO>> GetCountryDescription(CountryDescriptionQueryDTO requestQuery)
        {
            if (string.IsNullOrWhiteSpace(requestQuery.Language) || string.IsNullOrWhiteSpace(requestQuery.CountryCode))
            {
                return Result.Fail("Language And Country Code Required");
            }
            return await _unitOfWork.TourRepo.GetCountryDescription(requestQuery);
        }

        public async Task<Result<GetCountryInfoResponseDTO>> GetCountryInfoByIdAsync(string Id, GetLanuageQuery query)
        {

            var validationResult = await _validationService.ValidateAsync(query);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid CountryInfoId = guid;
            SupportedLanguage language = !string.IsNullOrWhiteSpace(query.Language) ? EnumsMapping.ToLanguageEnum(query.Language) : SupportedLanguage.en;

            var countryInfoSpec = new GetCountryInfoByIdSpecification(CountryInfoId, language);

            var countryInfo = await _unitOfWork.GetRepository<CountryInfo, Guid>().GetByIdAsync(countryInfoSpec);
            if (countryInfo is null)
            {
                return Result.Fail(new NotFoundError("This Country  Not Found"));
            }

            var mappedCountryInfo = countryInfo.FromEntityToDTO();

            return Result.Ok(mappedCountryInfo);
        }
    }
}
