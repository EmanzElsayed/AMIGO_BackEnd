

using Amigo.Application.Specifications.CountriesInfo;
using Amigo.Domain.Enum;

namespace Amigo.Application.Services.Admin
{
    public class AdminDestinationService(IValidationService _validationService,
                                    IUnitOfWork _unitOfWork,
                                    
                                    ImageCloudService _imageCloud) : IAdminDestinationService
    {
        public async Task<Result> CreateDestinationAsync(CreateDestinationRequestDTO requestDTO)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            CountryCode countryCode = EnumsMapping.ToCountryCodeEnum(requestDTO.CountryCode);
            Language language = EnumsMapping.ToLanguageEnum(requestDTO.Language);
            var countryInfo = await _unitOfWork.GetRepository<CountryInfo, Guid>().GetByIdAsync(new GetCountryByCountryCodeSpecification(countryCode,language));

            if (countryInfo is null)
            {
                return Result.Fail(new NotFoundError("This County Not Found"));
            }
            var destination = requestDTO.DestinationToEntity(countryInfo);

            try
            {
                await _unitOfWork.GetRepository<Destination, Guid>().AddAsync(destination);

                await _unitOfWork.SaveChangesAsync();


                return Result.Ok()
                                .WithSuccess(new Success("Destination Created Successfully")
                                .WithMetadata("StatusCode", (int)HttpStatusCode.Created));
            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);
            }
            
        }


        public async Task<Result> UpdateDestination(UpdateDestinationRequestDTO requestDTO, string Id)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid destinationId = guid;

            var _destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();
            var _bookingRepo = _unitOfWork.GetRepository<Booking, Guid>();


            var destination = await _destinationRepo.GetByIdAsync(new GetDestinationWithTranslationsSpecification(destinationId));

            if (destination is null)
            {
                return Result.Fail(new NotFoundError("This Destination Not Found"));
            }
            DestinationTranslation? translation = null;
            Language? languageEnum = null;

            if (requestDTO.Language is not null)
            {
                languageEnum = EnumsMapping.ToLanguageEnum(requestDTO.Language);
                translation = destination.Translations
                             .FirstOrDefault(t => t.Language == languageEnum);
            }


            


            if (!string.IsNullOrWhiteSpace(requestDTO.CountryCode) && languageEnum is not null)
            { 
                CountryCode countryCode = EnumsMapping.ToCountryCodeEnum(requestDTO.CountryCode);
                var countryInfo = await _unitOfWork.GetRepository<CountryInfo, Guid>().GetByIdAsync(new GetCountryByCountryCodeSpecification(countryCode, languageEnum.Value));

                if (countryInfo is null)
                {
                    return Result.Fail(new NotFoundError("This County Not Found"));
                }
                destination.CountryInfoId = countryInfo.Id;
                destination.CountryInfo = countryInfo;

            }


            // image logic
            if (!string.IsNullOrWhiteSpace(requestDTO.ImageUrl) )
            {
                destination.ImageUrl = requestDTO.ImageUrl;

                if (destination.ImagePublicId is not null)
                    _imageCloud.DeleteImage(destination.ImagePublicId);

                if (requestDTO.PublicId is not null)
                    destination.ImagePublicId = requestDTO.PublicId;
            }

            DestinationMapping.UpdateDestination(requestDTO, destination, translation, languageEnum);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return Result.Ok()
                               .WithSuccess(new Success("Destination Updated Successfully"));
            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);

            }

        }
        public async Task<Result> UpdateActivationDestinaion(UpdateActivationDestinationRequestDTO requestDTO, string Id)
        {
            var validationResult = await _validationService.ValidateAsync(requestDTO);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid destinationId = guid;

            var _destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();

            var destination = await _destinationRepo.GetByIdAsync(destinationId);

            if (destination is null)
            {
                return Result.Fail(new NotFoundError("This Destination Not Found"));
            }
            if (destination.IsActive == requestDTO.IsActive)
            {
                return Result.Ok()
                              .WithSuccess(new Success("Activation Status Already Updated Successfully"));

            }
            destination.IsActive = requestDTO.IsActive;

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return Result.Ok()
                               .WithSuccess(new Success("Activation Status Updated Successfully"));
            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);

            }
        }

        public async Task<Result> DeleteDestination(string Id)
        {
            if (!BusinessRules.TryCleanGuid(Id, out Guid guid))
                return Result.Fail("Invalid UUID");

            Guid destinationId = guid;

            var _destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();

            var destination = await _destinationRepo.GetByIdAsync(destinationId);


            if (destination is null)
            {
                return Result.Fail(new NotFoundError("This Destination Not Found"));
            }
            //is booked by people can't removed
            var _bookingRepo = _unitOfWork.GetRepository<Booking, Guid>();

            //var hasBookings = await _bookingRepo.AnyAsync(
            //                   new ActiveBookingsForDestinationSpecification(destinationId)
            //              );

            //if (hasBookings)
            //{


               
            //        return Result.Fail(new ConfilctError("Cannot delete destination  with active bookings"));
               

            //}
            _destinationRepo.Remove(destination);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                bool isDeleted = false;
                if (destination.ImagePublicId is not null)
                {
                    isDeleted = _imageCloud.DeleteImage(destination.ImagePublicId);
                }
                return Result.Ok()
                               .WithSuccess(new Success($"Destination Deleted Successfully is image deleted {isDeleted}"));
            }
            catch (Exception ex)
            {
                return FluentValidationExtension.FromException(details: ex.Message);

            }
        }


        public async Task<Result<PaginatedResponse<GetDestinationResponseDTO>>> GetAllDestinationAsync(GetAllDestinationQuery requestQuery)
        {
            var validationResult = await _validationService.ValidateAsync(requestQuery);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }
            var destinationRepo = _unitOfWork.GetRepository<Destination, Guid>();

            var destinationSpecification = new GetAllDestinationSpecification(requestQuery, true);
            var destinationData = await destinationRepo.GetAllAsync(destinationSpecification);

            var countDestinationSpecification = new CountGetAllDestinationSpecification(requestQuery, true);
            var countDestinationData = await destinationRepo.GetCountSpecificationAsync(countDestinationSpecification);

            Language? language = string.IsNullOrWhiteSpace(requestQuery.Language) ? null : EnumsMapping.ToLanguageEnum(requestQuery.Language);
            
            var mappedDestinationData = DestinationMapping.EntitiesToDestinations(destinationData,language);
            var paginatedResult = new PaginatedResponse<GetDestinationResponseDTO>
            {
                Data = mappedDestinationData,
                PageNumber = requestQuery.PageNumber,
                PageSize = requestQuery.PageSize,
                TotalItems = countDestinationData
            };
            return Result.Ok(paginatedResult);
        }

        public async Task<Result<GetDestinationResponseDTO>> GetDestinationByIdAsync(string Id, GetLanuageQuery requestQuery)
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
            var destinationSpecification = new AdminGetDestinationByIdSpecification(destinationId, requestQuery);

            var destinationData = await destinationRepo.GetByIdAsync(destinationSpecification);
            if (destinationData is null)
            {
                return Result.Fail(new NotFoundError($"This Destination Not Found"));
            }
            Language? language = string.IsNullOrWhiteSpace(requestQuery.Language) ? null : EnumsMapping.ToLanguageEnum(requestQuery.Language);

            var mappedDestinationData = DestinationMapping.EntityToAdminDestination(destinationData, language);
            return Result.Ok(mappedDestinationData);
        }

       
    }
}
