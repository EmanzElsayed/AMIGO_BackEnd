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
        Task<Result<PaginatedResponse<GetDestinationResponseDTO>>> GetAllDestinationAsync(GetAllDestinationQuery requestQuery, CancellationToken cancellationToken);
        Task<Result<GetDestinationResponseDTO>> GetDestinationByIdAsync(string destinationId, CancellationToken cancellationToken);

        Task<Result<PaginatedResponse<TopDestinationSummaryResponseDTO>>> GetTopDestinationsAsync(GetTopDestinationsQuery requestQuery, CancellationToken cancellationToken);
    }
}
