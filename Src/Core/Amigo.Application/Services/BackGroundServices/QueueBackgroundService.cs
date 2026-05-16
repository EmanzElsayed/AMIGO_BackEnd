using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.BackGroundServices
{
    public class QueueBackgroundService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        public QueueBackgroundService(
             IBackgroundTaskQueue queue,
             IServiceProvider serviceProvider)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _queue.DequeueAsync(stoppingToken);

               
                using var scope = _serviceProvider.CreateScope();

                await workItem(stoppingToken, scope.ServiceProvider);
            }
        }
    }
}
