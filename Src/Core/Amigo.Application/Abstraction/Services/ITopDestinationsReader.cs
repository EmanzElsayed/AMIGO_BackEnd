using Amigo.SharedKernal.DTOs.Destination;
using Amigo.SharedKernal.DTOs.Results;
using Amigo.SharedKernal.QueryParams;

namespace Amigo.Application.Abstraction.Services;

public interface ITopDestinationsReader
{
    Task<PaginatedResponse<TopDestinationSummaryResponseDTO>> GetTopAsync(GetTopDestinationsQuery query, CancellationToken cancellationToken = default);
}
