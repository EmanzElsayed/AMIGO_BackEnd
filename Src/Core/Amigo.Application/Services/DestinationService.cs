using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Amigo.Application.Specifications.BookingSpecification;
using Amigo.Application.Specifications.CountriesInfo;
using Amigo.Domain.DTO.CountryInfo;
using Amigo.Domain.Entities;

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

        public async Task<Result<GetDestinationByIdResponseDTO>> GetDestinationByIdAsync(string Id,string userType ,CancellationToken cancellationToken)
        {
           
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");
          
            Guid destinationId = guid;

            SupportedLanguage language = _currentUserService.Language;
            var user_type = ParseUserType(userType);
            var destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();
            var destinationSpecification = new GetDestinationByIdSpecification(destinationId ,language );
            
            var destinationData = await destinationRepo.GetByIdAsync(destinationSpecification, cancellationToken);
            if (destinationData is null)
            {
                return Result.Fail(new NotFoundError(_localizationService.Get("NotFoundDestination")));
            }
            var toursIds = await _unitOfWork.TourRepo.GetTourIdsWithDestinationId(destinationId, user_type);

            var reviews = await _unitOfWork.ReviewRepo.GetTourReviewSummariesAsync(toursIds);
            var averageRating = Math.Max(
                reviews != null && reviews.Any()
                    ? reviews.Average(r => r.Rate)
                    : 0,
                Constants.AverageReviewRating);
            var reviewsCount = (reviews != null && reviews.Any() ? reviews.Count() : 0 ) + Constants.ReviewCount;

            //var bookings = await _unitOfWork.GetRepository<Booking, Guid>().GetAllAsync(new GetBookingsByTourIdsSpecification(toursIds));
           
            var toursCount = toursIds.Count();
            var traveleresCount = Constants.TravelersCount + await _unitOfWork.PriceRepo.GetTravelersCount(toursIds);
            var mappedDestinationData = DestinationMapping.EntityToDestination(destinationData , language,averageRating,traveleresCount,reviewsCount,toursCount);

            return Result.Ok(mappedDestinationData);
        }

        public async Task<Result<PaginatedResponse<TopDestinationSummaryResponseDTO>>> GetTopDestinationsAsync(GetTopDestinationsQuery requestQuery,string? userType ,CancellationToken cancellationToken)
        {
            var validationResult = await _validationService.ValidateAsync(requestQuery);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var data = await _topDestinationsReader.GetTopAsync(requestQuery,userType);
            return Result.Ok(data).WithSuccess(new Success("Top destinations loaded successfully."));
        }

        private static UserType ParseUserType(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return UserType.Public;
            if (s.Equals("VIP", StringComparison.OrdinalIgnoreCase))
                return UserType.VIP;
            if (s.Equals("Standard", StringComparison.OrdinalIgnoreCase)
                || s.Equals("Public", StringComparison.OrdinalIgnoreCase))
                return UserType.Public;
            return UserType.Public;
        }

        public async Task<Result<GetCountryByIdResponseDTO>> GetCountryByIdAsync(string Id, string userType, CancellationToken cancellationToken)
        {
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid countryId = guid;

            SupportedLanguage language = _currentUserService.Language;
            var user_type = ParseUserType(userType);
            var countryRepo = _unitOfWork.GetRepository<CountryInfo, Guid>();
            var countrySpecification = new GetCountryInfoByIdSpecification(countryId, language);

            var countryData = await countryRepo.GetByIdAsync(countrySpecification, cancellationToken);
            if (countryData is null)
            {
                return Result.Fail("Not Found Country");
            }
            var toursIds = await _unitOfWork.TourRepo.GetTourIdsWithCountryId(countryId, user_type);

            var reviews = await _unitOfWork.ReviewRepo.GetTourReviewSummariesAsync(toursIds);
            var averageRating = Math.Max(
                reviews != null && reviews.Any()
                    ? reviews.Average(r => r.Rate)
                    : 0,
                Constants.AverageReviewRating);
            var reviewsCount = (reviews != null && reviews.Any() ? reviews.Count() : 0) + Constants.CountryReviewCount;

            var destinationCount = await _unitOfWork.TourRepo.GetDestinationCountWithCountryId(countryId);
            var toursCount = toursIds.Count();
            var traveleresCount = Constants.CountryTravelersCount + await _unitOfWork.PriceRepo.GetTravelersCount(toursIds);
            var mappedCountryData = CountryInfoMapping.EntityToCountry(countryData, language, averageRating, traveleresCount, reviewsCount, toursCount, destinationCount);

            return Result.Ok(mappedCountryData);
        }
    }

}
