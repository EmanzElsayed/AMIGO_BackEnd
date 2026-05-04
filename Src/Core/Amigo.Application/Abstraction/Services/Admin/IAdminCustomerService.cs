using Amigo.Domain.DTO.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Admin
{
    public interface IAdminCustomerService
    {
        // Get User 

        Task<Result<PaginatedResponse<AdminCustomerResponseDTO>>> GetCustomersAsync(GetAllCustomersQuery query);
        Task<Result<UpdateVipResponseDTO>> UpdateVipStatusAsync(string id, UpdateVipStatusRequestDTO request);
        Task<Result<GetCustomersDashboardResponseDTO>> GetadminCustomerDashboardAsync();

        //update User Status 
    }
}
