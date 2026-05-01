using Amigo.Application.Specifications.WebhookEventLogSpecification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services
{
    public class OutboxWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OutboxWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessBatch(stoppingToken);
                await Task.Delay(3000, stoppingToken);
            }
        }
        private async Task ProcessBatch(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();

            var unitOfWork = scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();

         
           // 1) get messages
            
            var outboxRepo = unitOfWork.GetRepository<OutboxMessage, Guid>();

            var messages = await outboxRepo.GetAllAsync(
                new GetPendingOutboxSpecification(DateTime.UtcNow, 50));

            if (!messages.Any())

                return;

            
            // 2 mark processing (bulk)
           
            foreach (var msg in messages)
                msg.Status = OutboxStatus.Processing;

            await unitOfWork.SaveChangesAsync(ct);

            
            // 3) process in parallel SAFELY
          
            await Parallel.ForEachAsync(messages, ct, async (msg, token) =>
            {
                using var innerScope = _scopeFactory.CreateScope();

                var innerUow = innerScope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

                var bookingService = innerScope.ServiceProvider
                    .GetRequiredService<IBookingService>();

                var outboxRepoInner =
                    innerUow.GetRepository<OutboxMessage, Guid>();

                try
                {
                    if (msg.Type == "PaymentSucceeded")
                    {
                        var paymentId = Guid.Parse(msg.Payload);

                        await bookingService.FinalizeBooking(paymentId);
                    }

                   
                    // reload entity (important)
                 
                    var entity = await outboxRepoInner.GetByIdAsync(msg.Id);

                    entity.Status = OutboxStatus.Completed;
                    entity.ProcessedAtUtc = DateTime.UtcNow;
                    entity.LastError = null;

                    await innerUow.SaveChangesAsync(token);
                }
                catch (Exception ex)
                {
                    var entity = await outboxRepoInner.GetByIdAsync(msg.Id);

                    entity.RetryCount++;

                    entity.Status = entity.RetryCount >= 10
                        ? OutboxStatus.Failed
                        : OutboxStatus.Pending;

                    entity.LastError = ex.Message;

                    entity.NextRetryAt = DateTime.UtcNow
                        .AddSeconds(Math.Pow(2, entity.RetryCount));

                    await innerUow.SaveChangesAsync(token);
                }
            });
        }

    }
}
