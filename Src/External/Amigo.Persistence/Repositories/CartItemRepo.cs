using Amigo.Domain.Abstraction.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.Repositories
{
    public class CartItemRepo(AmigoDbContext _dbContext)
        : GenericRepo<CartItem, Guid>(_dbContext), ICartItemRepo
    {
        public async Task BulkSoftDeleteByUserId(string userId)
        {
            await _dbContext.CartItems
                    .Where(x => x.Cart.UserId == userId)
                    .ExecuteUpdateAsync(s =>
                        s.SetProperty(x => x.IsDeleted, true));
        }
    }
}
