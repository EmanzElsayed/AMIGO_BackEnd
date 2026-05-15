using System.Linq.Expressions;
using System.Threading;

namespace Amigo.Domain.Abstraction.Repositories;

public interface IGenericRepo<TEntity,TKey> where TEntity : BaseEntity<TKey>
{
    //Task SaveChanges(CancellationToken cancellationToken);
    public Task<IEnumerable<TEntity>> GetAllAsync( CancellationToken cancellationToken = default);

    public Task<TEntity?> GetByIdAsync(TKey id,CancellationToken cancellationToken = default);
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    public void Update(TEntity entity);

    public void Remove(TEntity entity);


    #region region With specification
    public Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications,CancellationToken cancellationToken = default);

    public Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications, CancellationToken cancellationToken = default);


    public Task<int> GetCountSpecificationAsync(ISpecifications<TEntity, TKey> specifications, CancellationToken cancellationToken = default);


    public Task<bool> AnyAsync(ISpecifications<TEntity, TKey> spec, CancellationToken cancellationToken = default);

    Task<double?> MaxAsync(ISpecifications<TEntity, TKey> specifications, Expression<Func<TEntity, double>> selector, CancellationToken cancellationToken = default);

    #endregion

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task RemoveWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public void RemoveRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

   public Task<Dictionary<TKey, int>> GetGroupedCountAsync<TKey>(
               Expression<Func<TEntity, bool>> filter,
               Expression<Func<TEntity, TKey>> groupBy, CancellationToken cancellationToken = default);
}
