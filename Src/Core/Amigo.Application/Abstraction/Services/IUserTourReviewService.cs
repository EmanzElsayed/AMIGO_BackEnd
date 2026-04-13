using Amigo.SharedKernal.DTOs.Tour;

namespace Amigo.Application.Abstraction.Services;

public interface IUserTourReviewService
{
    Task<Result<TourReviewEligibilityDto>> GetEligibilityAsync(string? userId, Guid tourId);

    Task<Result<Guid>> SubmitReviewAsync(string userId, CreateUserTourReviewRequestDto request);
}
