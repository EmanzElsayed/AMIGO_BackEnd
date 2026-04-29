using Amigo.Domain.DTO.Destination;
using Amigo.SharedKernal.DTOs.Destination;
using Amigo.SharedKernal.DTOs.Results;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services.Admin
{
    public interface IAdminDestinationService
    {
        Task<Result> CreateDestinationAsync(CreateDestinationRequestDTO requestDTO);
        Task<Result> UpdateDestination(UpdateDestinationRequestDTO requestDTO, string Id);
        Task<Result> DeleteDestination(string Id);
        Task<Result> UpdateActivationDestinaion(UpdateActivationDestinationRequestDTO requestDTO, string Id);

        //some changes

        Task<Result<PaginatedResponse<GetDestinationResponseDTO>>> GetAllDestinationAsync(GetAllDestinationQuery requestQuery);
        Task<Result<GetDestinationResponseDTO>> GetDestinationByIdAsync(string destinationId, GetLanuageQuery requestQuery);


    }
}
