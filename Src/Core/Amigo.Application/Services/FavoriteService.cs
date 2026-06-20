using Amigo.Application.Abstraction.Services;
using Amigo.Application.Helpers;
using Amigo.Domain.Abstraction;
using Amigo.Domain.DTO.Favorite;
using Amigo.Domain.Entities;
using Amigo.Domain.Errors.BusinessErrors;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amigo.Application.Services;

public class FavoriteService(IUnitOfWork _unitOfWork, ICurrentUserService _currentUserService) : IFavoriteService
{
    public async Task<Result<List<FavoriteResponseDTO>>> GetFavoritesAsync(string userId)
    {
        var language = _currentUserService.Language;
        var favorites = await _unitOfWork.FavoritesRepo.GetUserFavoritesAsync(userId, language);
        foreach (var favorite in favorites)
        {
            favorite.TourSlug = SlugHelper.ToUrlSlug(favorite.Title);
            favorite.DestinationSlug = SlugHelper.ToUrlSlug(favorite.DestinationSlug);
        }
        return Result.Ok(favorites);
    }

    public async Task<Result<ToggleFavoriteResponseDTO>> AddFavoriteAsync(string userId, Guid tourId)
    {
        var exists = await _unitOfWork.FavoritesRepo.ExistsAsync(userId, tourId);
        if (exists)
        {
            return Result.Ok(new ToggleFavoriteResponseDTO { TourId = tourId, IsFavorite = true });
        }

        var tour = await _unitOfWork.GetRepository<Tour, Guid>().GetByIdAsync(tourId);
        if (tour == null || tour.IsDeleted)
        {
            return Result.Fail(new NotFoundError("Tour not found"));
        }

        var entity = new Favorites
        {
            UserId = userId,
            TourId = tourId,
            User = null!,
            Tour = null!
        };

        await _unitOfWork.FavoritesRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(new ToggleFavoriteResponseDTO { TourId = tourId, IsFavorite = true });
    }

    public async Task<Result<ToggleFavoriteResponseDTO>> RemoveFavoriteAsync(string userId, Guid tourId)
    {
        var row = await _unitOfWork.FavoritesRepo.GetByUserAndTourAsync(userId, tourId);
        if (row == null)
        {
            return Result.Ok(new ToggleFavoriteResponseDTO { TourId = tourId, IsFavorite = false });
        }

        row.SetIsDeleted(true);
        _unitOfWork.FavoritesRepo.Update(row);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(new ToggleFavoriteResponseDTO { TourId = tourId, IsFavorite = false });
    }
}
