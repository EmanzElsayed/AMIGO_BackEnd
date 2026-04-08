using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.DTO.Tour;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/tour")]
    [Authorize(Roles = "Admin")]
    public class AdminTourController(IAdminTourService _adminTourService) 
        :BaseController
    {
        [HttpPost]
        public async Task<IResultBase> CreateTour([FromBody] CreateTourRequestDTO requestDTO)
        {
           
            return await _adminTourService.CreateTourAsync(requestDTO);

        }

    }
}
