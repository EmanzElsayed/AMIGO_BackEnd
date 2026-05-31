using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.DTO.Destination;
using Amigo.Presentation.Attributes;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/destination")]
    [Authorize(Roles = "Admin")]
    public class AdminDestinationController( IServiceManager _serviceManager) : BaseController
    {
        [HttpPost]
        public async Task<IResultBase> CreateDestination([FromBody] CreateDestinationRequestDTO requestDTO)
        {
            return await _serviceManager.AdminDestinationService.CreateDestinationAsync(requestDTO);

        }
        [HttpGet]
        public async Task<IResultBase> GetAllDestination([FromQuery] GetAllDestinationQuery requestQuery)
        {
            return await _serviceManager.AdminDestinationService.GetAllDestinationAsync(requestQuery);

        }
        [HttpGet("{id}")]
        public async Task<IResultBase> GetDestinationById(string id, [FromQuery] GetLanuageQuery requestQuery)
        {
            return await _serviceManager.AdminDestinationService.GetDestinationByIdAsync(id, requestQuery);

        }



        [HttpPatch("{id}")]
        public async Task<IResultBase> UpdateDestinaion(string id, [FromBody] UpdateDestinationRequestDTO requestDTO)
        {

            return await _serviceManager.AdminDestinationService.UpdateDestination(requestDTO, id);

        }
        [HttpPatch("{id}/toggle")]
        public async Task<IResultBase> UpdateActivationDestinaion(string id, [FromBody] UpdateActivationDestinationRequestDTO requestDTO)
        {

            return await _serviceManager.AdminDestinationService.UpdateActivationDestinaion(requestDTO, id);

        }
        [HttpDelete("{id}")]
        public async Task<IResultBase> DeleteDestinaion(string id)
        {

            return await _serviceManager.AdminDestinationService.DeleteDestination(id);

        }
    }
}
