using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface ITourRepo
    {

        Task<Dictionary<Guid, string?>> GetFirstTourImagesAsync(
         IEnumerable<Guid> tourIds);
    }
}
