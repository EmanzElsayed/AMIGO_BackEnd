using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Destination;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/destination")]
    public class DestinationController(IDestinationService _destinationService):BaseController
    {
        [HttpPost]
        public async Task<IResultBase> CreateDestination([FromBody] CreateDestinationRequestDTO requestDTO)
        {
            return await _destinationService.CreateDestinationAsync(requestDTO);

        }
        [HttpGet]
        public async Task<IResultBase> GetAllDestination([FromQuery] GetAllDestinationQuery requestQuery)
        {
            return await _destinationService.GetAllDestinationAsync(requestQuery);

        }
    }
}
