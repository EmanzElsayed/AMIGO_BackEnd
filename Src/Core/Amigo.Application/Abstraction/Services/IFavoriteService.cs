using Amigo.Domain.DTO.Favorite;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amigo.Application.Abstraction.Services;

public interface IFavoriteService
{
    Task<Result<List<FavoriteResponseDTO>>> GetFavoritesAsync(string userId);
    Task<Result<ToggleFavoriteResponseDTO>> AddFavoriteAsync(string userId, Guid tourId);
    Task<Result<ToggleFavoriteResponseDTO>> RemoveFavoriteAsync(string userId, Guid tourId);
}
