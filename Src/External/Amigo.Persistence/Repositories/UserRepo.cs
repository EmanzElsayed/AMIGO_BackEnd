using Amigo.Domain.Abstraction;
using Amigo.Domain.Abstraction.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.Repositories
{
    public class UserRepo(AmigoDbContext _dbContext) : IUserRepo
    {
        public async Task<bool> AnyAsync(IUserSpecification specifications)
        {
            return await UserSpecificationEvaluator.CreateQuery(_dbContext.Set<ApplicationUser>(), specifications).AnyAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync(IUserSpecification specifications)
        {
            return await UserSpecificationEvaluator.CreateQuery(_dbContext.Set<ApplicationUser>(), specifications).ToListAsync();
        }

        public async Task<ApplicationUser?> GetByIdAsync(IUserSpecification specifications)
        {
            return await UserSpecificationEvaluator.CreateQuery(_dbContext.Set<ApplicationUser>(), specifications).FirstOrDefaultAsync();
        }

        public  async Task<int> GetCountSpecificationAsync(IUserSpecification specifications)
        {
            return await UserSpecificationEvaluator.CreateQuery(_dbContext.Set<ApplicationUser>(), specifications).CountAsync();
        }

        public void RemoveRange(IEnumerable<ApplicationUser> entities)
        
        => _dbContext.Set<ApplicationUser>().RemoveRange(entities);

        public async Task<List<string>> GetUserIdsInRoleAsync(string roleName)
        {
            var normalizedRole = roleName.ToUpper();

            var roleId = await _dbContext.Roles
                .AsNoTracking()
                .Where(x => x.NormalizedName == normalizedRole)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(roleId))
                return [];

            return await _dbContext.UserRoles
                .AsNoTracking()
                .Where(x => x.RoleId == roleId)
                .Select(x => x.UserId)
                .Distinct()
                .ToListAsync();
        }

    }
}
