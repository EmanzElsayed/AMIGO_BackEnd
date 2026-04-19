using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.DTO.Tour;
using Amigo.Domain.Enum;
using Amigo.SharedKernal.QueryParams;
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
        [HttpPatch("{id}")]
        public async Task<IResultBase> UpdateTour([FromBody] UpdateTourRequestDTO requestDTO, string id)
        {

            return await _adminTourService.UpdateTourAsync(requestDTO, id);

        }
        [HttpGet]
        public async Task<IResultBase> GetTours([FromQuery] GetAllAdminTourQuery requestDTO)
        {

            return await _adminTourService.GetAllToursAsync(requestDTO);

        }
        [HttpGet("{id}")]
        public async Task<IResultBase> GetTourById(string id, [FromBody] GetTourByIdRequestDTO requestDTO)
        {

            return await _adminTourService.GetTourById(id , requestDTO);

        }
    }
}
