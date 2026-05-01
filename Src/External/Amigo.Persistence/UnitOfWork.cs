using Amigo.Domain.Abstraction;
using Amigo.Domain.Abstraction.Repositories;
using Amigo.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence
{
    public class UnitOfWork(AmigoDbContext _dbContext) : IUnitOfWork
    {
        private readonly Dictionary<string, object> _repositories = [];

        public IGenericRepo<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var typeName = typeof(TEntity).Name;

            if (_repositories.ContainsKey(typeName))
            {
                return (IGenericRepo<TEntity, TKey>)_repositories[typeName];
            }
            else
            {
                var repo = new GenericRepo<TEntity, TKey>(_dbContext);
                _repositories.Add(typeName, repo);
                return repo;

            }
        }


        private ISlotsRepo? _slotsRepo;

        public ISlotsRepo SlotsRepo
            => _slotsRepo ??= new SlotsRepo(_dbContext);


        private ICartItemRepo? _cartItemsRepo;

        public ICartItemRepo CartItemsRepo
            => _cartItemsRepo ??= new CartItemRepo(_dbContext);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
                => await _dbContext.SaveChangesAsync(ct);

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }
        public IExecutionStrategy CreateExecutionStrategy()
        {
            return  _dbContext.Database.CreateExecutionStrategy();
        }
    }
}
