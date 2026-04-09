using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.DTO.Price;
using Amigo.Domain.DTO.Tour;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Presentation.Controllers.Admin
{
    [Route("api/v1/admin/price")]
    [Authorize(Roles = "Admin")]
    public class AdminPriceController(IAdminPriceService _adminPriceService) : BaseController
    {
        [HttpPost]
        public async Task<IResultBase> CreatePrice([FromBody] CreatePriceRequestDTO requestDTO)
        {

            return await _adminPriceService.CreatePriceAsync(requestDTO);

        }
    }
}
