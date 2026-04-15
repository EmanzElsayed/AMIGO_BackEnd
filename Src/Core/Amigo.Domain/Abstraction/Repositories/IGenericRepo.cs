using System.Linq.Expressions;

namespace Amigo.Domain.Abstraction.Repositories;

public interface IGenericRepo<TEntity,TKey> where TEntity : BaseEntity<TKey>
{
    //Task SaveChanges(CancellationToken cancellationToken);
    public Task<IEnumerable<TEntity>> GetAllAsync();

    public Task<TEntity?> GetByIdAsync(TKey id);
    public Task AddAsync(TEntity entity);

    public void Update(TEntity entity);

    public void Remove(TEntity entity);


    #region region With specification
    public Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications);

    public Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications);


    public Task<int> GetCountSpecificationAsync(ISpecifications<TEntity, TKey> specifications);


    public Task<bool> AnyAsync(ISpecifications<TEntity, TKey> spec);

    Task<double?> MaxAsync(ISpecifications<TEntity, TKey> specifications, Expression<Func<TEntity, double>> selector);

    #endregion

    public Task AddRangeAsync(IEnumerable<TEntity> entities);

    Task RemoveWhereAsync(Expression<Func<TEntity, bool>> predicate);

    public void RemoveRange(IEnumerable<TEntity> entities);
}
