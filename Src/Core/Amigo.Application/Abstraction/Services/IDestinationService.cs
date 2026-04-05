using Amigo.Domain.DTO.Destination;
using Amigo.Domain.Entities;
using Amigo.SharedKernal.DTOs.Destination;
using Amigo.SharedKernal.DTOs.Results;
using Amigo.SharedKernal.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IDestinationService
    {
        Task<Result> CreateDestinationAsync(CreateDestinationRequestDTO requestDTO);
        Task<Result<PaginatedResponse<GetDestinationResponseDTO>>> GetAllDestinationAsync(GetAllDestinationQuery requestQuery , bool isAdmin);
        Task<Result<GetDestinationResponseDTO>> GetDestinationByIdAsync(string destinationId , bool isAdmin , GetDestinationByIdQuery requestQuery);


        Task<Result> UpdateDestination(UpdateDestinationRequestDTO requestDTO,string Id);
        Task<Result> DeleteDestination( string Id);

        Task<Result> UpdateActivationDestinaion(UpdateActivationDestinationRequestDTO requestDTO,string Id);



    }
}
