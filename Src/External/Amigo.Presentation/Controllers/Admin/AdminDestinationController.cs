using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.DTO.Destination;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/destination")]
    [Authorize(Roles = "Admin")]
    public class AdminDestinationController(IAdminDestinationService _destinationService) : BaseController
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
        [HttpGet("{id}")]
        public async Task<IResultBase> GetDestinationById(string id, [FromQuery] GetDestinationByIdQuery requestQuery)
        {
            return await _destinationService.GetDestinationByIdAsync(id, requestQuery);

        }



        [HttpPatch("{id}")]
        public async Task<IResultBase> UpdateDestinaion(string id, [FromBody] UpdateDestinationRequestDTO requestDTO)
        {

            return await _destinationService.UpdateDestination(requestDTO, id);

        }
        [HttpPatch("{id}/toggle")]
        public async Task<IResultBase> UpdateActivationDestinaion(string id, [FromBody] UpdateActivationDestinationRequestDTO requestDTO)
        {

            return await _destinationService.UpdateActivationDestinaion(requestDTO, id);

        }
        [HttpDelete("{id}")]
        public async Task<IResultBase> DeleteDestinaion(string id)
        {

            return await _destinationService.DeleteDestination(id);

        }
    }
}
