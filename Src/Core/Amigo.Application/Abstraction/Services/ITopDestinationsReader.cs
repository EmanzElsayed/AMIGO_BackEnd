using Amigo.SharedKernal.DTOs.Destination;
using Amigo.SharedKernal.QueryParams;

namespace Amigo.Application.Abstraction.Services;

public interface ITopDestinationsReader
{
    Task<IReadOnlyList<TopDestinationSummaryResponseDTO>> GetTopAsync(GetTopDestinationsQuery query, CancellationToken cancellationToken = default);
}
