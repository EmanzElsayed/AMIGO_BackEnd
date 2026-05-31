using Amigo.Application.Abstraction.Services;
using Amigo.Application.Abstraction.Services.Admin;
using Amigo.Domain.DTO.User;

using Amigo.SharedKernal.QueryParams;
using Microsoft.AspNetCore.Authorization;

namespace Amigo.Presentation.Controllers.Admin;

[Route("api/v1/admin/customer")]
[Authorize(Roles = "Admin")]
public class AdminCustomerController(
    IServiceManager _serviceManager
  
    ) : BaseController
{
    [HttpGet]
    public async Task<IResultBase> GetCustomers([FromQuery] GetAllCustomersQuery query)
    {
        return await _serviceManager.AdminCustomerService.GetCustomersAsync(query);
    }
    [HttpGet("dashboard")]
    public async Task<IResultBase> GetCustomersDashboard()
    {
        return await _serviceManager.AdminCustomerService.GetadminCustomerDashboardAsync();
    }
    [HttpGet("dashboard/activities")]
    public async Task<IResultBase> GetDashboardActivities([FromQuery] GetAllAdminTourQuery query)
    {
        return await _serviceManager.AdminCustomerService.GetDashboardActivitiesAsync(query);
    }
    [HttpPatch("{id}/vip-status")]
    public async Task<IResultBase> UpdateVipStatus(
         string id,
        [FromBody] UpdateVipStatusRequestDTO request)
    {
        return await _serviceManager.AdminCustomerService.UpdateVipStatusAsync(id, request);
    }


}