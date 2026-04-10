using Amigo.Domain.DTO.AvailableSlots;
using Amigo.SharedKernal.DTOs.TourSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.Admin
{
    public class AdminAvailableSlotsService(IValidationService _validationService,
                                    IUnitOfWork _unitOfWork,
                                    IAvailableSlotsMapping _availableSlotsMapping) : IAdminAvailableSlotsService
    {
        //public async Task<Result> CreateAvailableSlotsAsync(CreateAvailableSlotsRequestDTO requestDTO)
        //{
        //    var validationResult = await _validationService.ValidateAsync(requestDTO);
        //    if (!validationResult.IsSuccess)
        //    {
        //        return validationResult;
        //    }
        //    var tourSchedule = await _unitOfWork.GetRepository<TourSchedule, Guid>().GetByIdAsync(requestDTO.TourScheduleId);
        //    if (tourSchedule is null)
        //    {
        //        return Result.Fail(new NotFoundError("This Tour Schedule Not Found"));

        //    }
        //    var availableSlots = _availableSlotsMapping.AvailableSlotsDTOToEntity(requestDTO, tourSchedule);
        //    try
        //    {
        //        await _unitOfWork.GetRepository<AvailableSlots, Guid>().AddAsync(availableSlots);
        //        await _unitOfWork.SaveChangesAsync();

        //        return Result.Ok()
        //                    .WithSuccess(new Success("Available Slots Created Successfully")
        //                    .WithMetadata("StatusCode", (int)HttpStatusCode.Created));
        //    }
        //    catch (Exception ex)
        //    {

        //        return FluentValidationExtension.FromException(details: ex.Message);

        //    }
        //}
    }
}
