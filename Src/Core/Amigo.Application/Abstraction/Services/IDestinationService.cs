using Amigo.Domain.DTO.Destination;
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
        Task<Result<PaginatedResponse<GetTranslationDestinationResponseDTO>>> GetAllDestinationAsync(GetAllDestinationQuery requestQuery);
    }
}
