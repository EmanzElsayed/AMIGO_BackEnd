using Amigo.Application.Abstraction.Services;
using Amigo.Application.Validators.Review;
using Amigo.Domain.DTO.Review;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.SharedKernal.DTOs.Tour;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Amigo.Presentation.Controllers.Admin;

[Route("api/v1/admin/tours/reviews")]
[Authorize(Roles = "Admin")]
public class AdminTourReviewController( IServiceManager _serviceManager ) : BaseController
{
    [HttpDelete("{reviewId:guid}")]
    public async Task<IResultBase> DeleteReview([FromRoute] Guid reviewId)
    {
        var effectiveUserId = string.Empty;
        return await _serviceManager.UserTourReviewService.DeleteReviewAsync(effectiveUserId, reviewId);
    }

    [HttpPost]
    public async Task<IResultBase> CreateReview([FromBody] AdminAddReviewsDTO requestDTO)
    {
        return await _serviceManager.AdminTourService.AddReview(requestDTO);
    }
}
