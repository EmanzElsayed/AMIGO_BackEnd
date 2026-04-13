namespace Amigo.Application.Abstraction.Services;

public interface ITourReviewEligibilityReader
{
    Task<bool> CanUserWriteReviewAsync(string userId, Guid tourId, CancellationToken cancellationToken = default);
}
