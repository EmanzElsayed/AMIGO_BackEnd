using Amigo.Application.Abstraction.Services;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Amigo.Presentation.Controllers.User
{
    [Route("api/v1/user/travelers")]
    public class TravelersController(IServiceManager _serviceManager) :BaseController
    {
        [HttpGet]
        public async Task<IResultBase> GetTravelers([FromQuery]GetAllTravelersQuery query)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _serviceManager.TravelersService.GetAllTravelers(userId,query);
        }
        [HttpGet("{id}")]
        public async Task<IResultBase> GetTravelers(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _serviceManager.TravelersService.GetTravelerById(userId, id);
        }

    }
}
