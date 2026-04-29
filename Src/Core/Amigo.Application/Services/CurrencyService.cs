using Amigo.Application.Abstraction.Services;
using Amigo.Application.Specifications.CurrencySpecification;
using Amigo.Domain.Abstraction;
using Amigo.Domain.DTO.Currency;
using Amigo.Domain.Entities;
using Amigo.Domain.Entities.TranslationEntities;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.DTOs.Currency;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Amigo.Application.Services
{
    public  class CurrencyService (IUnitOfWork _unitOfWork, IValidationService _validationService) : ICurrencyService
    {

        public async Task<Result<List<GetCurrencyResponseDTO>>> GetAllCurrencyAsync(GetAllCurrencyQuery requestQuery)
        {
            var validationResult = await _validationService.ValidateAsync(requestQuery);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            Language language = !string.IsNullOrWhiteSpace(requestQuery.Language) ? EnumsMapping.ToLanguageEnum(requestQuery.Language) : Language.en;


            var CurrencySpec = new GetAllCurrencySpecification(requestQuery,language);

            var currencies = await _unitOfWork.GetRepository<Currency, Guid>().GetAllAsync(CurrencySpec);
            var mappedCurrencies = currencies.FromEntitiesToDTOs();

           
            return Result.Ok(mappedCurrencies);
        }

        public  async Task<Result<GetCurrencyResponseDTO>> GetCurrencyByIdAsync(string Id, GetLanuageQuery query)
        {
            var validationResult = await _validationService.ValidateAsync(query);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid CurrencyId = guid;

            Language language = !string.IsNullOrWhiteSpace(query.Language) ? EnumsMapping.ToLanguageEnum(query.Language) : Language.en;

            var _currencyRepo = _unitOfWork.GetRepository<Currency, Guid>();

            var currency = await _currencyRepo.GetByIdAsync(new GetCurrencyWithQuerySpecification(CurrencyId , language));

            if (currency is null)
            {
                return Result.Fail(new NotFoundError("This Currency Not Found"));
            }

            var mappedCurrency = currency.FromEntityToDTO();
            
            return Result.Ok(mappedCurrency);

        }

        public async Task<Result> UpdateCurrency(UpdateCurrencyRequestDTO requestDTO, string Id)
        {
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid CurrencyId = guid;

            var _currencyRepo = _unitOfWork.GetRepository<Currency, Guid>();

            var currency = await _currencyRepo.GetByIdAsync(new GetCurrencyWithTranslationSpecification(CurrencyId));

            if (currency is null)
            {
                return Result.Fail(new NotFoundError("This Currency Not Found"));
            }

            CurrencyTranslation? translation = null;
            Language? languageEnum = null;

            if (requestDTO.Language is not null)
            {
                languageEnum = EnumsMapping.ToLanguageEnum(requestDTO.Language);
                translation = currency.Translations
                             .FirstOrDefault(t => t.Language == languageEnum);
            }

            if (!string.IsNullOrWhiteSpace(requestDTO.Name) && !string.IsNullOrWhiteSpace(requestDTO.Language))
            {
                if (translation is null)
                {
                    // add new language
                    currency.Translations.Add(new CurrencyTranslation
                    {
                        Language = languageEnum.Value,
                        Name = requestDTO.Name,
                    });
                }
                else
                {
                    //  update existing language
                    translation.Name = requestDTO.Name;
                }
            }
            if (!string.IsNullOrWhiteSpace(requestDTO.CurrencyCode))
            { 
                currency.CurrencyCode =  EnumsMapping.ToEnum<CurrencyCode>(requestDTO.CurrencyCode, false);

            }
            if (!string.IsNullOrWhiteSpace(requestDTO.Icon))
            { 
                currency.Icon = requestDTO.Icon;
            }
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return Result.Ok()
                               .WithSuccess(new Success("Currency Updated Successfully"));
            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);

            }
        }
        public async Task<Result> CreateCurrencyAsync(CreateCurrencyRequestDTO requestDTO)
        {
            Language language = EnumsMapping.ToLanguageEnum(requestDTO.Language);
            CurrencyCode code = EnumsMapping.ToEnum<CurrencyCode>(requestDTO.CurrencyCode, false);
            var currency = new Currency()
            {
                Id = Guid.NewGuid(),
                CurrencyCode = code,
                Translations = new List<CurrencyTranslation>
                {
                    new CurrencyTranslation
                    {
                        Id = Guid.NewGuid(),
                        Name = requestDTO.Name,
                        Language = language,
                    }

                }
            };
            if (!string.IsNullOrWhiteSpace(requestDTO.Icon)) currency.Icon = requestDTO.Icon;

            var strategy = _unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await _unitOfWork.GetRepository<Currency, Guid>().AddAsync(currency);
                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Result.Ok()
                                 .WithSuccess(new Success("Currency Created Successfully")
                                 .WithMetadata("StatusCode", (int)HttpStatusCode.Created));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return FluentValidationExtension.FromException(details: ex.Message);
                }
            });
        }

    }
}
