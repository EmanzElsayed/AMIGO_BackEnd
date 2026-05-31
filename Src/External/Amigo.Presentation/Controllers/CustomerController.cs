using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.User;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/customer")]
    public class CustomerController( IServiceManager _serviceManager ) :BaseController
    {
        [HttpPost("continue-with-email")]
        public async Task<IResultBase> ContinueWithEmail([FromBody] CreateAccountRequestDTO requestDTO)
        {
            return await _serviceManager.UserService.ContinueWithEmail(requestDTO);
        }

    }
}
