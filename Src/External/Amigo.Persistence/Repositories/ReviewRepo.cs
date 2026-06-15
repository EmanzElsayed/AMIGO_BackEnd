using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.DTO.Tour;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.Repositories
{
    public class ReviewRepo(AmigoDbContext _dbContext): IReviewRepo
    {
        public async Task<List<TourReviewRowDto>>
        GetTourReviewSummariesAsync(
        IReadOnlyCollection<Guid> tourIds,
        CancellationToken cancellationToken = default)
        {
            return await _dbContext.Reviews
                .AsNoTracking()
                .Where(r =>
                    tourIds.Contains(r.TourId) &&
                    !r.IsDeleted)

                 .Select(r => new TourReviewRowDto
                 {
                     TourId = r.TourId,
                     Rate = r.Rate
                 })
            .ToListAsync(cancellationToken);
        }

        public async Task<List<decimal>>
        GetTourReviewRatesAsync(
            Guid tourId,
        CancellationToken cancellationToken = default)
        {
            return await _dbContext.Reviews
                .AsNoTracking()
                .Where(r =>
                    r.TourId == tourId &&
                    !r.IsDeleted)

                 .Select(r => r.Rate

                 ).ToListAsync(cancellationToken);
            
        }
    }
}
