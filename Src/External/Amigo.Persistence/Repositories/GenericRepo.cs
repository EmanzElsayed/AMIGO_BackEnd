using Amigo.Domain.Abstraction;
using Amigo.Domain.Abstraction.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Amigo.Persistence.Repositories;

public class GenericRepo<TEntity, TKey>(AmigoDbContext _dbContext) 
    : IGenericRepo<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    //public async Task SaveChanges(CancellationToken cancellationToken)
    //{
    //    await dbContext.SaveChangesAsync(cancellationToken);
    //}

    public async Task AddAsync(TEntity entity)
           => await _dbContext.Set<TEntity>().AddAsync(entity);


    public async Task<IEnumerable<TEntity>> GetAllAsync()
        => await _dbContext.Set<TEntity>().ToListAsync();




    public async Task<TEntity?> GetByIdAsync(TKey id)
        => await _dbContext.Set<TEntity>().FindAsync(id);




    public void Remove(TEntity entity)
        => _dbContext.Set<TEntity>().Remove(entity);


    public void Update(TEntity entity)
        => _dbContext.Set<TEntity>().Update(entity);


    #region Specifications

    public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications)
    {
        return await SpeceficationEvaluator.CreateQuery<TEntity, TKey>(_dbContext.Set<TEntity>(), specifications).ToListAsync();
    }
    public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications)
    {
        return await SpeceficationEvaluator.CreateQuery<TEntity, TKey>(_dbContext.Set<TEntity>(), specifications).FirstOrDefaultAsync();
    }

    public async Task<int> GetCountSpecificationAsync(ISpecifications<TEntity, TKey> specifications)
    {
        return await SpeceficationEvaluator.CreateQuery<TEntity, TKey>(_dbContext.Set<TEntity>(), specifications).CountAsync();
    }

    public async Task<bool> AnyAsync(ISpecifications<TEntity, TKey> spec)
    {
        return await SpeceficationEvaluator
            .CreateQuery(_dbContext.Set<TEntity>(), spec)
            .AnyAsync();
    }
    #endregion

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {

        await _dbContext.Set<TEntity>().AddRangeAsync(entities);
    }
    public async Task RemoveWhereAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = await _dbContext.Set<TEntity>()
            .Where(predicate)
            .ToListAsync();

        _dbContext.Set<TEntity>().RemoveRange(entities);
    }
}
