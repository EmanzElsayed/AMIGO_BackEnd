using Amigo.Domain.DTO.Price;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface IPriceRepo

    {
        Task<List<FlatSummeryPriceDTO>>
       GetTourPriceSummariesAsync(
       IReadOnlyCollection<Guid> tourIds,
        UserType allowedUserType,
       CancellationToken cancellationToken = default);
    }
}
