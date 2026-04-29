namespace Amigo.Application.Services
{
    public class DestinationService(IValidationService _validationService,
                                    IUnitOfWork _unitOfWork ,
                                    IDestinationMapping _destinationMapping ,
                                    ImageCloudService _imageCloud,
                                    ITopDestinationsReader _topDestinationsReader) : IDestinationService
    {

    

        public async Task<Result<PaginatedResponse<GetDestinationResponseDTO>>> GetAllDestinationAsync(GetAllDestinationQuery requestQuery)
        {
            var validationResult = await _validationService.ValidateAsync(requestQuery);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            var destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();

            var destinationSpecification = new GetAllDestinationSpecification(requestQuery,false);
            var destinationData = await destinationRepo.GetAllAsync(destinationSpecification);

            var countDestinationSpecification = new CountGetAllDestinationSpecification(requestQuery , false);
            var countDestinationData = await destinationRepo.GetCountSpecificationAsync(countDestinationSpecification);

            var mappedDestinationData = _destinationMapping.EntitiesToDestinations(destinationData);
            var paginatedResult = new PaginatedResponse<GetDestinationResponseDTO>
            {
                Data = mappedDestinationData,
                PageNumber = requestQuery.PageNumber,
                PageSize = requestQuery.PageSize,
                TotalItems = countDestinationData
            };
            return Result.Ok(paginatedResult);
        }

        public async Task<Result<GetDestinationResponseDTO>> GetDestinationByIdAsync(string Id,  GetLanuageQuery requestQuery)
        {
            var validationResult = await _validationService.ValidateAsync(requestQuery);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");
          
            Guid destinationId = guid;

            var destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();
            var destinationSpecification = new GetDestinationByIdSpecification(destinationId ,requestQuery );
            
            var destinationData = await destinationRepo.GetByIdAsync(destinationSpecification);
            if (destinationData is null)
            {
                return Result.Fail(new NotFoundError($"This Destination Not Found"));
            }
            var mappedDestinationData = _destinationMapping.EntityToDestination(destinationData , requestQuery.Language);
            return Result.Ok(mappedDestinationData);
        }

        public async Task<Result<IReadOnlyList<TopDestinationSummaryResponseDTO>>> GetTopDestinationsAsync(GetTopDestinationsQuery requestQuery)
        {
            var validationResult = await _validationService.ValidateAsync(requestQuery);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var data = await _topDestinationsReader.GetTopAsync(requestQuery);
            return Result.Ok(data).WithSuccess(new Success("Top destinations loaded successfully."));
        }
    }
}
