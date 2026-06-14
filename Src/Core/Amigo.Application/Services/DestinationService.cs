using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Amigo.Application.Specifications.BookingSpecification;

namespace Amigo.Application.Services
{
    public class DestinationService(IValidationService _validationService,
                                    IUnitOfWork _unitOfWork ,
                                    ICurrentUserService _currentUserService,
                                    ITopDestinationsReader _topDestinationsReader,
                                     ILocalizationService _localizationService) 
                : IDestinationService
    {

    

        public async Task<Result<PaginatedResponse<GetDestinationResponseDTO>>> GetAllDestinationAsync(GetAllDestinationQuery requestQuery, CancellationToken cancellationToken)
        {
            var validationResult = await _validationService.ValidateAsync(requestQuery);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            var destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();

            SupportedLanguage language = _currentUserService.Language;

            var destinationSpecification = new GetAllDestinationSpecification(requestQuery,false,language);
            var destinationData = await destinationRepo.GetAllAsync(destinationSpecification, cancellationToken);

            var countDestinationSpecification = new CountGetAllDestinationSpecification(requestQuery , false,language);
            var countDestinationData = await destinationRepo.GetCountSpecificationAsync(countDestinationSpecification, cancellationToken);

            

            var mappedDestinationData = DestinationMapping.EntitiesToDestinations(destinationData, language);
            var paginatedResult = new PaginatedResponse<GetDestinationResponseDTO>
            {
                Data = mappedDestinationData,
                PageNumber = requestQuery.PageNumber,
                PageSize = requestQuery.PageSize,
                TotalItems = countDestinationData
            };
            return Result.Ok(paginatedResult);
        }

        public async Task<Result<GetDestinationByIdResponseDTO>> GetDestinationByIdAsync(string Id, CancellationToken cancellationToken)
        {
           
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");
          
            Guid destinationId = guid;

            SupportedLanguage language = _currentUserService.Language;

            var destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();
            var destinationSpecification = new GetDestinationByIdSpecification(destinationId ,language );
            
            var destinationData = await destinationRepo.GetByIdAsync(destinationSpecification, cancellationToken);
            if (destinationData is null)
            {
                return Result.Fail(new NotFoundError(_localizationService.Get("NotFoundDestination")));
            }
            var toursIds = await _unitOfWork.TourRepo.GetTourIdsWithDestinationId(destinationId);

            var reviews = await _unitOfWork.ReviewRepo.GetTourReviewSummariesAsync(toursIds);
            var averageRating = Math.Max(
                reviews != null && reviews.Any()
                    ? reviews.Average(r => r.Rate)
                    : 0,
                Constants.AverageReviewRating);
            var reviewsCount = (reviews != null && reviews.Any() ? reviews.Count() : 0 ) + Constants.ReviewCount;

            var bookings = await _unitOfWork.GetRepository<Booking, Guid>().GetAllAsync(new GetBookingsByTourIdsSpecification(toursIds));
           
            var traveleresCount = Constants.TravelersCount + bookings?.Sum(b => b.Travelers?.Count()) ?? 0;
            var mappedDestinationData = DestinationMapping.EntityToDestination(destinationData , language,averageRating,traveleresCount,reviewsCount);
           

            return Result.Ok(mappedDestinationData);
        }

        public async Task<Result<PaginatedResponse<TopDestinationSummaryResponseDTO>>> GetTopDestinationsAsync(GetTopDestinationsQuery requestQuery, CancellationToken cancellationToken)
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
