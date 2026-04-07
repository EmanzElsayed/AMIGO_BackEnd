using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Destination;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.User
{
    [Route("api/v1/user/destination")]
    
    public class DestinationController(IDestinationService _destinationService) : BaseController
    {
       
        [HttpGet]
        public async Task<IResultBase> GetAllDestination([FromQuery] GetAllDestinationQuery requestQuery)
        {
            return await _destinationService.GetAllDestinationAsync(requestQuery);

        }
        [HttpGet("{id}")]
        public async Task<IResultBase> GetDestinationById(string id, [FromQuery] GetDestinationByIdQuery requestQuery)
        {
            return await _destinationService.GetDestinationByIdAsync(id, requestQuery);

        }



       
       
    }
}
