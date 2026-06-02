using Amigo.Domain.DTO.Tour;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface IReviewRepo
    {
        Task<List<TourReviewRowDto>>
        GetTourReviewSummariesAsync(
          IReadOnlyCollection<Guid> tourIds,
          CancellationToken cancellationToken = default);
    }
}
