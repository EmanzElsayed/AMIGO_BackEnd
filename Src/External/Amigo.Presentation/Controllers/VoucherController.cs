using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Payment;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/voucher")]
    public class VoucherController( IServiceManager _serviceManager):BaseController
    {
        [HttpGet("validate")]
        public async Task<IResultBase> validateVoucher([FromQuery] string token)
        {
            return await _serviceManager.VoucherService.ValidateVoucher(token);
        }

    }
}
