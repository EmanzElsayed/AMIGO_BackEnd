using Amigo.Domain.DTO.Cancellation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface ICancellationRepo
    {
        Task<List<TourCancellationRowDto>>
    GetFreeCancellationLookupAsync(
        IReadOnlyCollection<Guid> tourIds,
        CancellationToken cancellationToken = default);
        Task<bool>
      GetIsFreeCancellationAsync(
      Guid tourId,
      CancellationToken cancellationToken = default);
    }
}
