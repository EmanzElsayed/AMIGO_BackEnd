using Amigo.Domain.DTO.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Admin
{
    public interface IAdminCustomerService
    {
        // Get Customer 

        Task<Result<PaginatedResponse<AdminCustomerResponseDTO>>> GetCustomersAsync(GetAllCustomersQuery query);
        Task<Result<UpdateVipResponseDTO>> UpdateVipStatusAsync(string id, UpdateVipStatusRequestDTO request);
        Task<Result<GetCustomersDashboardResponseDTO>> GetadminCustomerDashboardAsync();

        //update Customer Status 
    }
}
