using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Customer;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/customer")]
    public class CustomerController(ICustomerService _customerService) :BaseController
    {
        [HttpPost("continue-with-email")]
        public async Task<IResultBase> ContinueWithEmail([FromBody] CreateAccountRequestDTO requestDTO)
        {
            return await _customerService.ContinueWithEmail(requestDTO);
        }

    }
}
