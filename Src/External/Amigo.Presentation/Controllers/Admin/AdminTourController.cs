using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.DTO.Tour;
using Amigo.Domain.Enum;
using Amigo.Presentation.Attributes;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/tour")]
    [Authorize(Roles = "Admin")]
    public class AdminTourController( IServiceManager _serviceManager ) 
        :BaseController
    {
        [HttpPost]
        public async Task<IResultBase> CreateTour([FromBody] CreateTourRequestDTO requestDTO)
        {
           
            return await _serviceManager.AdminTourService.CreateTourAsync(requestDTO);

        }
        [HttpPatch("{id}")]
        public async Task<IResultBase> UpdateTour([FromBody] UpdateTourRequestDTO requestDTO, string id)
        {

            return await _serviceManager.AdminTourService.UpdateTourAsync(requestDTO, id);

        }
        [HttpGet]

        public async Task<IResultBase> GetTours([FromQuery] GetAllAdminTourQuery requestDTO)
        {

            return await _serviceManager.AdminTourService.GetAllToursAsync(requestDTO);

        }
        [HttpGet("{id}")]

        public async Task<IResultBase> GetTourById(string id, [FromQuery] GetTourByIdRequestDTO requestDTO)
        {

            return await _serviceManager.AdminTourService.GetTourById(id , requestDTO);

        }

        // not updated yet

        //[HttpGet("stats")]
        //public async Task<IResultBase> GetActivityStats()
        //{
        //    return await _serviceManager.AdminTourService.GetActivityStatsAsync();
        //}

        //    private static string EscapeLikePattern(string value)
        //    {
        //        return value
        //            .Replace("\\", "\\\\")
        //            .Replace("%", "\\%")
        //            .Replace("_", "\\_");
        //    }
        //}

    }
}
