using Amigo.Application.BackgroundTasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amigo.Application.BackgroundTasks
{
    
    public sealed class TranslationWorkerService : BackgroundService
    {
        private const int WorkerCount = 1;

        private readonly IBackgroundTaskQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TranslationWorkerService> _log;

        public TranslationWorkerService(
            IBackgroundTaskQueue queue,
            IServiceScopeFactory scopeFactory,
            ILogger<TranslationWorkerService> log)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _log.LogInformation(
                "[TranslationWorker] Started with {WorkerCount} worker(s).", WorkerCount);

            var workers = Enumerable
                .Range(1, WorkerCount)
                .Select(id => RunWorkerLoopAsync(id, stoppingToken));

            await Task.WhenAll(workers);

            _log.LogInformation("[TranslationWorker] All workers stopped.");
        }

        private async Task RunWorkerLoopAsync(int workerId, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Func<IServiceProvider, CancellationToken, Task>? workItem = null;

                try
                {
                    workItem = await _queue.DequeueAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    _log.LogDebug("[TranslationWorker:{Id}] Starting job.", workerId);

                    await workItem(scope.ServiceProvider, stoppingToken);

                    _log.LogDebug("[TranslationWorker:{Id}] Job completed.", workerId);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    _log.LogInformation(
                        "[TranslationWorker:{Id}] Job cancelled due to app shutdown.", workerId);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex,
                        "[TranslationWorker:{Id}] Unhandled error in job.", workerId);
                }
            }
        }
    }
}