using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.DTO.Customer;

using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Authorization;

namespace Amigo.Presentation.Controllers.Admin;

[Route("api/v1/admin/customer")]
[Authorize(Roles = "Admin")]
public class AdminCustomerController(
    IAdminCustomerService _adminCustomerService
    ) : BaseController
{
    [HttpGet]
    public async Task<IResultBase> GetCustomers([FromQuery] GetAllCustomersQuery query)
    {
        return await _adminCustomerService.GetCustomersAsync(query);
    }
    [HttpGet("dashboard")]
    public async Task<IResultBase> GetCustomersDashboard()
    {
        return await _adminCustomerService.GetadminCustomerDashboardAsync();
    }
    [HttpPatch("{id}/vip-status")]
    public async Task<IResultBase> UpdateVipStatus(
         string id,
        [FromBody] UpdateVipStatusRequestDTO request)
    {
        return await _adminCustomerService.UpdateVipStatusAsync(id, request);
    }


}