using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Payment;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers
{
    [Route("api/v1/voucher")]
    public class VoucherController(IVoucherService voucherService):BaseController
    {
        [HttpGet("validate")]
        public async Task<IResultBase> validateVoucher([FromBody]string token)
        {
            return await voucherService.ValidateVoucher(token);
        }

    }
}
