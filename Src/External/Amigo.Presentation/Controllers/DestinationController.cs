using Amigo.Application.Abstraction.Services;
using Amigo.Domain.DTO.Destination;
using Amigo.Domain.Errors.BusinessErrors;
using Amigo.SharedKernal.QueryParams;
using FluentResults;

namespace Amigo.Presentation.Controllers.User
{
    [Route("api/v1/user/destination")]

    public class DestinationController(
        IDestinationService _destinationService,
        IDestinationSlugResolver _slugResolver) : BaseController
    {
        [HttpGet]
        public async Task<IResultBase> GetAllDestination([FromQuery] GetAllDestinationQuery requestQuery)
        {
            return await _destinationService.GetAllDestinationAsync(requestQuery);
        }

        [HttpGet("by-slug/{slug}")]
        public async Task<IResultBase> GetDestinationBySlug(string slug, [FromQuery] GetDestinationByIdQuery requestQuery)
        {
            var id = await _slugResolver.ResolveDestinationIdAsync(slug);
            if (id is null)
                return Result.Fail(new NotFoundError("Destination not found for this link."));
            return await _destinationService.GetDestinationByIdAsync(id.Value.ToString(), requestQuery);
        }

        [HttpGet("{id:guid}")]
        public async Task<IResultBase> GetDestinationById(string id, [FromQuery] GetDestinationByIdQuery requestQuery)
        {
            return await _destinationService.GetDestinationByIdAsync(id, requestQuery);
        }

        [HttpGet("top")]
        public async Task<IResultBase> GetTopDestinations([FromQuery] GetTopDestinationsQuery requestQuery)
        {
            return await _destinationService.GetTopDestinationsAsync(requestQuery);
        }
    }
}
