using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.DTO.Refund;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/cancellation")]
    [Authorize(Roles = "Admin")]
    public class AdminCancellationController(IAdminCancellationService _adminCancellationService):BaseController
    {
        [HttpGet]
        public async Task<IResultBase> getAllCancellationRequest([FromQuery] GetAllAdminCancellationRequestQuery requestQuery)
        {
            return await _adminCancellationService.GetAllCancellationRequestsAsync(requestQuery);
        }
        [HttpPost("{id}/approve")]
        public async Task<IResultBase> ApproveCancellationRequest(string id)
        {
            return await _adminCancellationService.ApproveCancellationRequest(id);
        }
        [HttpPost("{id}/reject")]
        public async Task<IResultBase> RejectCancellationRequest(string id , [FromBody] RejectCancellationRequestDTO requestDTO)
        {
            return await _adminCancellationService.RejectCancellationRequest(id, requestDTO);
        } 
    }
}
