using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface ICartItemRepo
    {
        Task BulkSoftDeleteByUserId(string userId);
    }
}
