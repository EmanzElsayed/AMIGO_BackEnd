using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.DTO.Cancellation;
using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.Repositories
{
    public class CancellationRepo(AmigoDbContext _dbContext) : ICancellationRepo
    {
       public async Task<List<TourCancellationRowDto>>
        GetFreeCancellationLookupAsync(
        IReadOnlyCollection<Guid> tourIds,
        CancellationToken cancellationToken = default)
        {
            return await _dbContext.Cancelations
                .AsNoTracking()
                .Where(c =>
                    tourIds.Contains(c.TourId) &&
                    !c.IsDeleted)
               .Select(c => new TourCancellationRowDto
               {
                   TourId = c.TourId,
                   IsFree = c.CancelationPolicyType == CancelationPolicyType.Free
               })
            .ToListAsync(cancellationToken);
        }

        public async Task<bool>
        GetIsFreeCancellationAsync(
        Guid tourId,
        CancellationToken cancellationToken = default)
        {
            return  await _dbContext.Cancelations
               
                .AnyAsync(c =>
                    c.TourId == tourId &&  c.CancelationPolicyType == CancelationPolicyType.Free &&
                    !c.IsDeleted);
               
        }
    }
}
