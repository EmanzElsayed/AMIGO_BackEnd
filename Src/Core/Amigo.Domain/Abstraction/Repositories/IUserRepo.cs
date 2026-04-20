using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction.Repositories
{
    public interface IUserRepo
    {
        public Task<ApplicationUser?> GetByIdAsync(IUserSpecification specifications);

        public Task<IEnumerable<ApplicationUser>> GetAllAsync(IUserSpecification specifications);


        public Task<int> GetCountSpecificationAsync(IUserSpecification specifications);


        public Task<bool> AnyAsync(IUserSpecification spec);
        public void RemoveRange(IEnumerable<ApplicationUser> entities);

        Task<List<string>> GetUserIdsInRoleAsync(string roleName);

    }
}
