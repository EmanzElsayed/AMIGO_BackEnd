using Amigo.Domain.DTO.Price;
using Amigo.Domain.DTO.TourSchedule;
using Amigo.SharedKernal.DTOs.Results;
using Amigo.SharedKernal.DTOs.Tour;
using Amigo.SharedKernal.QueryParams;

namespace Amigo.Application.Abstraction.Services;

public interface IUserTourCatalogService
{
    Task<Result<PaginatedResponse<UserTourListItemDto>>> GetToursAsync(GetUserToursQuery query);

    Task<Result<IEnumerable<string>>> GetTourCategoriesAsync(Guid destinationId, string? language);

    Task<Result<MaxDurationHoursResponseDto>> GetMaxDurationHoursForDestinationAsync(Guid destinationId);

    Task<Result<MaxPriceResponseDto>> GetMaxPriceForDestinationAsync(Guid destinationId);

    Task<Result<UserTourDetailDto>> GetTourByPublicPathAsync(GetTourByPublicPathQuery query, string? userType, string? currentUserId = null);

    Task<Result<IEnumerable<UserTrendingTourItemDto>>> GetTrendingToursAsync(string? language, string? currency, string? userType,  string? countryCode,int take = 10);

    Task<Result<List<UserTourPriceTierDto>>> GetPriceByActivityTypeAsync(string id,PiceWithActivityTypeRequestQuery requestDTO, string? userType);
    Task<Result<UserTourScheduleDetailDTO>> GetTourScheduleDetails(string Id, string? userType);


    Task<Result<UserTourReviewDTO>> GetTourReviews(string Id, string? currentUserId );

}
