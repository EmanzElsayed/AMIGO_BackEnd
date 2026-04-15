using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Price;
using Amigo.SharedKernal.DTOs.Price;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminPriceService(IValidationService _validationService,
                                    IUnitOfWork _unitOfWork,
                                    IPriceMapping _priceMapping) : IAdminPriceService
    {
        //public async Task<Result<CreatePriceResponseDTO>> CreatePriceAsync(CreatePriceRequestDTO requestDTO)
        //{
        //    var validationResult = await _validationService.ValidateAsync(requestDTO);
        //    if (!validationResult.IsSuccess)
        //    {
        //        return validationResult;
        //    }

        //    var tour = await _unitOfWork.GetRepository<Tour, Guid>().GetByIdAsync(requestDTO.TourId);
        //    if (tour is null)
        //    {
        //        return Result.Fail(new NotFoundError("This Tour Not Found"));

        //    }

        //    var price = _priceMapping.PriceDTOToEntity(requestDTO, tour);

        //    try { 
        //        await _unitOfWork.GetRepository<Price,Guid>().AddAsync(price);
        //        await _unitOfWork.SaveChangesAsync();

        //        return Result.Ok(new CreatePriceResponseDTO(price.RetailPrice))
        //                    .WithSuccess(new Success("Price Created Successfully")
        //                    .WithMetadata("StatusCode", (int)HttpStatusCode.Created));
        //    }
        //    catch (Exception ex)
        //    {

        //        return FluentValidationExtension.FromException(details: ex.Message);

        //    }
        //}
        public Task UpdatePricesAsync(
             Tour tour,
             List<UpdatePriceRequestDTO> prices,
             Language? language)
        {
            if (tour == null || language is null || prices == null || prices.Count == 0)
                return Task.CompletedTask;

            var lang = language.Value;

            var existingPricesDict = tour.Prices
                .Where(p => p.Id != Guid.Empty)
                .ToDictionary(p => p.Id, p => p);

            foreach (var dto in prices)
            {
                Price price;

                if (dto.Id.HasValue &&
                    existingPricesDict.TryGetValue(dto.Id.Value, out var existingPrice))
                {
                    price = existingPrice;

                    if (dto.Cost is not null)
                        price.Cost = dto.Cost.Value;

                    if (dto.Discount is not null)
                        price.Discount = dto.Discount.Value;
                }
                else
                {
                    price = new Price
                    {
                        Cost = dto.Cost ?? 0,
                        Discount = dto.Discount ?? 0,
                        UserType = dto.UserType ?? UserType.Public,
                        TourId = tour.Id,
                        Translations = new List<PriceTranslation>()
                    };

                    tour.Prices.Add(price);
                }

                var translation = price.Translations
                    .FirstOrDefault(t => t.Language == lang);

                if (translation == null)
                {
                    if (!price.Translations.Any(t => t.Language == lang))
                    {
                        price.Translations.Add(new PriceTranslation
                        {
                            
                            Language = lang,
                            Type = dto.Type ?? string.Empty,
                            Price = price
                        });
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dto.Type))
                        translation.Type = dto.Type;
                }
            }

            return Task.CompletedTask;
        }
    }
}
