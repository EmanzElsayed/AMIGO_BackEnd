using Amigo.Application.Abstraction.Services;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.SharedKernal.DTOs.Tour;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Amigo.Presentation.Controllers.User;

[Route("api/v1/user/tours")]
public class UserTourReviewController(IUserTourReviewService reviews) : BaseController
{
    [EnableRateLimiting("token")]

    [HttpGet("{tourId:guid}/review-eligibility")]
    [AllowAnonymous]
    public async Task<IResultBase> GetReviewEligibility([FromRoute] Guid tourId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await reviews.GetEligibilityAsync(userId, tourId);
    }
    [EnableRateLimiting("token")]

    [HttpPost("reviews")]
    [Authorize]
    public async Task<IResultBase> SubmitReview([FromBody] CreateUserTourReviewRequestDto body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        return await reviews.SubmitReviewAsync(userId, body);
    }
    [EnableRateLimiting("token")]

    [HttpPost("reviews/{reviewId:guid}/helpful")]
    [AllowAnonymous]
    public async Task<IResultBase> MarkAsHelpful([FromRoute] Guid reviewId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        return await reviews.MarkAsHelpfulAsync(reviewId, userId, ip);
    }
    [EnableRateLimiting("token")]

    [HttpPut("reviews/{reviewId:guid}")]
    [Authorize]
    public async Task<IResultBase> UpdateReview([FromRoute] Guid reviewId, [FromBody] UpdateUserTourReviewRequestDto body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        return await reviews.UpdateReviewAsync(userId, reviewId, body);
    }
    [EnableRateLimiting("token")]

    [HttpDelete("reviews/{reviewId:guid}")]
    [Authorize]
    public async Task<IResultBase> DeleteReview([FromRoute] Guid reviewId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        return await reviews.DeleteReviewAsync(userId, reviewId);
    }
}
