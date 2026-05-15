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
        IDestinationService _destinationService,
        IDestinationSlugResolver _slugResolver) : BaseController
    {
        [EnableRateLimiting("token")]
        [HttpGet]
        [Cache(900)]
        public async Task<IResultBase> GetAllDestination([FromQuery] GetAllDestinationQuery requestQuery, CancellationToken cancellationToken)
        {
            return await _destinationService.GetAllDestinationAsync(requestQuery, cancellationToken);
        }

        [EnableRateLimiting("token")]
        [HttpGet("by-slug/{slug}")]
        public async Task<IResultBase> GetDestinationBySlug(string slug, CancellationToken cancellationToken)
        {
            var id = await _slugResolver.ResolveDestinationIdAsync(slug, cancellationToken);
            if (id is null)
                return Result.Fail(new NotFoundError("Destination not found for this link."));
            return await _destinationService.GetDestinationByIdAsync(id.Value.ToString(), cancellationToken);
        }
        [EnableRateLimiting("token")]
        [HttpGet("{id:guid}")]
        public async Task<IResultBase> GetDestinationById(string id, CancellationToken cancellationToken)
        {
            return await _destinationService.GetDestinationByIdAsync(id, cancellationToken);
        }

        [EnableRateLimiting("token")]
        [HttpGet("top")]
        [Cache(900)]
        public async Task<IResultBase> GetTopDestinations([FromQuery] GetTopDestinationsQuery requestQuery, CancellationToken cancellationToken)
        {
            return await _destinationService.GetTopDestinationsAsync(requestQuery, cancellationToken);
        }
    }
}
