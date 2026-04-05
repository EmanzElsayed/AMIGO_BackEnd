using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Destination;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/destination")]
    public class DestinationController(IDestinationService _destinationService):BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IResultBase> CreateDestination([FromBody] CreateDestinationRequestDTO requestDTO)
        {
            return await _destinationService.CreateDestinationAsync(requestDTO);

        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IResultBase> GetAllDestination([FromQuery] GetAllDestinationQuery requestQuery)
        {
            var isAdmin = User.IsInRole("Admin");
            return await _destinationService.GetAllDestinationAsync(requestQuery,isAdmin);

        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IResultBase> GetDestinationById(string id, [FromQuery] GetDestinationByIdQuery requestQuery)
        {
            var isAdmin = User.IsInRole("Admin");
            return await _destinationService.GetDestinationByIdAsync(id,isAdmin,requestQuery);

        }

       

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IResultBase> UpdateDestinaion(string id , [FromBody] UpdateDestinationRequestDTO requestDTO)
        {
            
            return await _destinationService.UpdateDestination(requestDTO,id);

        }
        [HttpPatch("{id}/toggle")]
        [Authorize(Roles = "Admin")]
        public async Task<IResultBase> UpdateActivationDestinaion(string id, [FromBody] UpdateActivationDestinationRequestDTO requestDTO)
        {

            return await _destinationService.UpdateActivationDestinaion(requestDTO, id);

        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IResultBase> DeleteDestinaion(string id)
        {

            return await _destinationService.DeleteDestination(id);

        }
    }
}
