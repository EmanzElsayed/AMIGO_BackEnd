using Amigo.Application.Abstraction.Services;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.SharedKernal.DTOs.Tour;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Amigo.Presentation.Controllers.User;

[Route("api/v1/user/tours")]
public class UserTourReviewController( IServiceManager _serviceManager ) 
    : BaseController
{
    [EnableRateLimiting("token")]

    [HttpGet("{tourId:guid}/review-eligibility")]
    [AllowAnonymous]
    public async Task<IResultBase> GetReviewEligibility([FromRoute] Guid tourId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _serviceManager.UserTourReviewService.GetEligibilityAsync(userId, tourId);
    }
    [EnableRateLimiting("token")]

    [HttpPost("reviews")]
    [Authorize]
    public async Task<IResultBase> SubmitReview([FromBody] CreateUserTourReviewRequestDto body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        return await _serviceManager.UserTourReviewService.SubmitReviewAsync(userId, body);
    }
    [EnableRateLimiting("token")]

    [HttpPost("reviews/{reviewId:guid}/helpful")]
    [AllowAnonymous]
    public async Task<IResultBase> MarkAsHelpful([FromRoute] Guid reviewId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        return await _serviceManager.UserTourReviewService.MarkAsHelpfulAsync(reviewId, userId, ip);
    }
    [EnableRateLimiting("token")]

    [HttpPut("reviews/{reviewId:guid}")]
    [Authorize]
    public async Task<IResultBase> UpdateReview([FromRoute] Guid reviewId, [FromBody] UpdateUserTourReviewRequestDto body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        return await _serviceManager.UserTourReviewService.UpdateReviewAsync(userId, reviewId, body);
    }
    [EnableRateLimiting("token")]

    [HttpDelete("reviews/{reviewId:guid}")]
    [Authorize]
    public async Task<IResultBase> DeleteReview([FromRoute] Guid reviewId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Fail(new UnauthorizedError("Not authenticated"));

        return await _serviceManager.UserTourReviewService.DeleteReviewAsync(userId, reviewId);
    }
}
