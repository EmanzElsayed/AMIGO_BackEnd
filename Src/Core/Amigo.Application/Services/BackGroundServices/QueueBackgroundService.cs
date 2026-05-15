using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Services.BackGroundServices
{
    public class QueueBackgroundService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;

        public QueueBackgroundService(IBackgroundTaskQueue queue)
        {
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _queue.DequeueAsync(stoppingToken);
                await workItem(stoppingToken);
            }
        }
    }
}
