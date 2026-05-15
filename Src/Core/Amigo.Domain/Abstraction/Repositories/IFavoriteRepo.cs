using Amigo.Domain.DTO.Favorite;
using Amigo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amigo.Domain.Abstraction.Repositories;

public interface IFavoriteRepo
{
    Task<List<FavoriteResponseDTO>> GetUserFavoritesAsync(string userId);
    Task<bool> ExistsAsync(string userId, Guid tourId);
    Task AddAsync(Favorites favorite);
    Task<Favorites?> GetByUserAndTourAsync(string userId, Guid tourId);
    void Update(Favorites favorite);
}
