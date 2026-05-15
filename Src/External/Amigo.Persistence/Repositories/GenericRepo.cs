using Amigo.Domain.Abstraction;
using Amigo.Domain.Abstraction.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Amigo.Persistence.Repositories;

public class GenericRepo<TEntity, TKey>(AmigoDbContext _dbContext) 
    : IGenericRepo<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
           => await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);


    public async Task<IEnumerable<TEntity>> GetAllAsync( CancellationToken cancellationToken = default)
        => await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);




    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        => await _dbContext.Set<TEntity>().FindAsync(id, cancellationToken);




    public void Remove(TEntity entity)
        => _dbContext.Set<TEntity>().Remove(entity);


    public void Update(TEntity entity)
        => _dbContext.Set<TEntity>().Update(entity);


    #region Specifications

    public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications, CancellationToken cancellationToken = default)
    {
        return await SpeceficationEvaluator.CreateQuery<TEntity, TKey>(_dbContext.Set<TEntity>(), specifications).AsNoTracking().ToListAsync(cancellationToken);
    }
    public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications, CancellationToken cancellationToken = default)
    {
        return await SpeceficationEvaluator.CreateQuery<TEntity, TKey>(_dbContext.Set<TEntity>(), specifications).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetCountSpecificationAsync(ISpecifications<TEntity, TKey> specifications, CancellationToken cancellationToken = default)
    {
        return await SpeceficationEvaluator.CreateQuery<TEntity, TKey>(_dbContext.Set<TEntity>(), specifications).CountAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(ISpecifications<TEntity, TKey> spec, CancellationToken cancellationToken = default)
    {
        return await SpeceficationEvaluator
            .CreateQuery(_dbContext.Set<TEntity>(), spec)
            .AnyAsync(cancellationToken);
    }

    public async Task<double?> MaxAsync(ISpecifications<TEntity, TKey> specifications, Expression<Func<TEntity, double>> selector, CancellationToken cancellationToken = default)
    {
        var q = SpeceficationEvaluator.CreateQuery(_dbContext.Set<TEntity>(), specifications);
        if (!await q.AnyAsync())
            return null;
        return await q.MaxAsync(selector, cancellationToken);
    }
    #endregion

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {

        await _dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
    }

    public async Task RemoveWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Set<TEntity>()
            .Where(predicate)
            .ToListAsync(cancellationToken);

        _dbContext.Set<TEntity>().RemoveRange(entities);
    }
    public void RemoveRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => _dbContext.Set<TEntity>().RemoveRange(entities);


    public async Task<Dictionary<TKey, int>> GetGroupedCountAsync<TKey>(
                Expression<Func<TEntity, bool>> filter,
                Expression<Func<TEntity, TKey>> groupBy, CancellationToken cancellationToken = default)
    where TKey : notnull
    {
        return await _dbContext.Set<TEntity>()
            .Where(filter)
            .GroupBy(groupBy)
            .Select(g => new
            {
                Key = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);
    }

}
