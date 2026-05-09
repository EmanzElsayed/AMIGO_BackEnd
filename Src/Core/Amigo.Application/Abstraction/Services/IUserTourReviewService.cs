using Amigo.SharedKernal.DTOs.Tour;

namespace Amigo.Application.Abstraction.Services;

public interface IUserTourReviewService
{
    Task<Result<TourReviewEligibilityDto>> GetEligibilityAsync(string? userId, Guid tourId);

    Task<Result<Guid>> SubmitReviewAsync(string userId, CreateUserTourReviewRequestDto request);
    Task<Result<int>> MarkAsHelpfulAsync(Guid reviewId, string? userId, string? ipAddress = null);
    Task<Result> UpdateReviewAsync(string userId, Guid reviewId, UpdateUserTourReviewRequestDto request);
    Task<Result> DeleteReviewAsync(string userId, Guid reviewId);
}
