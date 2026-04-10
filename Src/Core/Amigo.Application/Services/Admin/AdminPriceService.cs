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
    }
}
