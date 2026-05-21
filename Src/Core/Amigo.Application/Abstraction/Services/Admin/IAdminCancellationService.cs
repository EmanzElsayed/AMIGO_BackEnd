using Amigo.Domain.DTO.Refund;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Admin
{
    public interface IAdminCancellationService
    {
        Task<Result<PaginatedResponse<GetAllCancellationRequestsDTO>>> GetAllCancellationRequestsAsync(GetAllAdminCancellationRequestQuery requestQuery);

        Task<Result> ApproveCancellationRequest(string Id);
        Task<Result> RejectCancellationRequest(string Id , RejectCancellationRequestDTO requestDTO);

    }
}
