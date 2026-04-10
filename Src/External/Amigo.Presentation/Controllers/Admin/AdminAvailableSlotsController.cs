using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.DTO.AvailableSlots;
using Amigo.Domain.DTO.TourSchedule;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/available-slots")]
    [Authorize(Roles = "Admin")]
    public class AdminAvailableSlotsController(IAdminAvailableSlotsService _adminAvailableSlotsService) : BaseController
    {
        //[HttpPost]
        //public async Task<IResultBase> CreateAvailableSlots([FromBody] CreateAvailableSlotsRequestDTO requestDTO)
        //{

        //    return await _adminAvailableSlotsService.CreateAvailableSlotsAsync(requestDTO);

        //}
    }
}
