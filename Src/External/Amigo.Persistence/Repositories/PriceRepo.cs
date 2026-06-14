using Amigo.Domain.Abstraction.Repositories;
using Amigo.Domain.DTO.Price;
using Amigo.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.Repositories
{
    public class PriceRepo(AmigoDbContext _dbContext) 
        :  IPriceRepo
    {
        public async Task<List<FlatSummeryPriceDTO>>
        GetTourPriceSummariesAsync(
        IReadOnlyCollection<Guid> tourIds,
        UserType allowedUserType,
        CancellationToken cancellationToken = default)
        {
            return await _dbContext.Prices
                .AsNoTracking()
                .Where(p =>
                    tourIds.Contains(p.TourId) &&
                    !p.IsDeleted &&
                    (p.UserType & allowedUserType) == allowedUserType
                    &&
                    (p.IsMainActivityType == null || p.IsMainActivityType == true)
                    && (p.SpecialDate == null)
                    )
              
                .Select(p => new FlatSummeryPriceDTO
                {
                    TourId = p.TourId,
                    RetailPrice = p.RetailPrice,
                    CostPrice = p.Cost
                   
                }).ToListAsync(cancellationToken);

        }
    }
}
