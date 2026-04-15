using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Currency;
using Amigo.Domain.DTO.Destination;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/currency")]
    [Authorize(Roles = "Admin")]
    public class AdminCurrencyController(ICurrencyService _currencyService) : BaseController
    {
        [HttpPost]
        public async Task<IResultBase> CreateCurrency([FromBody] CreateCurrencyRequestDTO requestDTO)
        {
            return await _currencyService.CreateCurrencyAsync(requestDTO);

        }

        [HttpPatch("{id}")]
        public async Task<IResultBase> UpdateCurrency(string id, [FromBody] UpdateCurrencyRequestDTO requestDTO)
        {

            return await _currencyService.UpdateCurrency(requestDTO, id);

        }
    }
}
