using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Application.Services.Admin;
using Amigo.Domain.DTO.Price;
using Amigo.Domain.DTO.TourSchedule;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/tour-schedule")]
    [Authorize(Roles = "Admin")]
    public class AdminTourScheduleController(IAdminTourScheduleService _adminTourScheduleService) : BaseController
    {
        //[HttpPost]
        //public async Task<IResultBase> CreateTourSchedule([FromBody] CreateTourScheduleRequestDTO requestDTO)
        //{

        //    return await _adminTourScheduleService.CreateTourScheduleAsync(requestDTO);

        //}
    }
}
