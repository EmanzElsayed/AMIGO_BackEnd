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
        public async Task<IResultBase> GetAllDestination([FromQuery] GetAllDestinationQuery requestQuery)
        {
            return await _destinationService.GetAllDestinationAsync(requestQuery);
        }

        [EnableRateLimiting("token")]
        [HttpGet("by-slug/{slug}")]
        [Cache(900)]
        public async Task<IResultBase> GetDestinationBySlug(string slug, [FromQuery] GetLanuageQuery requestQuery)
        {
            var id = await _slugResolver.ResolveDestinationIdAsync(slug);
            if (id is null)
                return Result.Fail(new NotFoundError("Destination not found for this link."));
            return await _destinationService.GetDestinationByIdAsync(id.Value.ToString(), requestQuery);
        }
        [EnableRateLimiting("token")]
        [HttpGet("{id:guid}")]
        [Cache(900)]
        public async Task<IResultBase> GetDestinationById(string id, [FromQuery] GetLanuageQuery requestQuery)
        {
            return await _destinationService.GetDestinationByIdAsync(id, requestQuery);
        }
        [EnableRateLimiting("token")]
        [HttpGet("top")]
        [Cache(900)]
        public async Task<IResultBase> GetTopDestinations([FromQuery] GetTopDestinationsQuery requestQuery)
        {
            return await _destinationService.GetTopDestinationsAsync(requestQuery);
        }
    }
}
