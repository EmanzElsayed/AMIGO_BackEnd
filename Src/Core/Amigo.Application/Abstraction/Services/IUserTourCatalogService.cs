using Amigo.SharedKernal.DTOs.Results;
using Amigo.SharedKernal.DTOs.Tour;
using Amigo.SharedKernal.QueryParams;

namespace Amigo.Application.Abstraction.Services;

public interface IUserTourCatalogService
{
    Task<Result<PaginatedResponse<UserTourListItemDto>>> GetToursAsync(GetUserToursQuery query);

    Task<Result<IEnumerable<string>>> GetTourCategoriesAsync(Guid destinationId, string? language);

    Task<Result<MaxDurationHoursResponseDto>> GetMaxDurationHoursForDestinationAsync(Guid destinationId);

    Task<Result<UserTourDetailDto>> GetTourByPublicPathAsync(GetTourByPublicPathQuery query, string? userType);

    Task<Result<IEnumerable<UserTrendingTourItemDto>>> GetTrendingToursAsync(string? language, string? currency, string? userType, int take = 10);
}
