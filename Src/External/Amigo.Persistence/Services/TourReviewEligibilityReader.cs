using Amigo.Application.Abstraction.Services;
using Amigo.Domain.Entities;
using Amigo.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Amigo.Persistence.Services;

public class TourReviewEligibilityReader(AmigoDbContext db) : ITourReviewEligibilityReader
{
    public async Task<bool> CanUserWriteReviewAsync(
        string userId,
        Guid tourId,
        CancellationToken cancellationToken = default)
    {
        return true;
    }
}
