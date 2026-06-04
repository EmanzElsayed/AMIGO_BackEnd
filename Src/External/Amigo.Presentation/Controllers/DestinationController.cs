using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.Presentation.Attributes;
using Amigo.SharedKernal.QueryParams;
using FluentResults;
using Microsoft.AspNetCore.RateLimiting;

namespace Amigo.Presentation.Controllers.User
{
    [Route("api/v1/user/destination")]

    public class DestinationController(
       IServiceManager _serviceManager) : BaseController
    {
        [EnableRateLimiting("token")]
        [HttpGet]
        //[Cache(1800)]
        public async Task<IResultBase> GetAllDestination([FromQuery] GetAllDestinationQuery requestQuery, CancellationToken cancellationToken)
        {
            return await _serviceManager.DestinationService.GetAllDestinationAsync(requestQuery, cancellationToken);
        }

        [EnableRateLimiting("token")]
        [HttpGet("by-slug/{slug}")]
        public async Task<IResultBase> GetDestinationBySlug(string slug, CancellationToken cancellationToken)
        {
            var id = await _serviceManager.DestinationSlugResolver.ResolveDestinationIdAsync(slug, cancellationToken);
            if (id is null)
                return Result.Fail(new NotFoundError("Destination not found for this link."));
            return await _serviceManager.DestinationService.GetDestinationByIdAsync(id.Value.ToString(), cancellationToken);
        }
        [EnableRateLimiting("token")]
        [HttpGet("{id:guid}")]
        public async Task<IResultBase> GetDestinationById(string id, CancellationToken cancellationToken)
        {
            return await _serviceManager.DestinationService.GetDestinationByIdAsync(id, cancellationToken);
        }

        [EnableRateLimiting("token")]
        [HttpGet("top")]
        //[Cache(1800)]
        public async Task<IResultBase> GetTopDestinations([FromQuery] GetTopDestinationsQuery requestQuery, CancellationToken cancellationToken)
        {
            return await _serviceManager.DestinationService.GetTopDestinationsAsync(requestQuery, cancellationToken);
        }
    }
}
