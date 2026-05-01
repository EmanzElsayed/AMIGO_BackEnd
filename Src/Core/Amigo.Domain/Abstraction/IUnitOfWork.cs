using Amigo.Domain.Abstraction.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Domain.Abstraction
{
    public interface IUnitOfWork
    {
        public IGenericRepo<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>;

        ISlotsRepo SlotsRepo { get; }
        ICartItemRepo CartItemsRepo { get; }

        public Task<int> SaveChangesAsync(CancellationToken ct = default);
        public Task<IDbContextTransaction> BeginTransactionAsync();

        public IExecutionStrategy CreateExecutionStrategy();

    }
}
